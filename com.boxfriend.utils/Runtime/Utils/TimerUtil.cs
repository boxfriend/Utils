using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boxfriend.Utils
{
	/// <summary>
	/// Simple timer class that will call an action on timer complete
	/// </summary>
	public class TimerUtil
	{
		public static TimerUtil Timer (Action action, float time, string name = "TimerObject")
		{
			GameObject timerObj = new GameObject(name, typeof(TimerMonoBehaviour));
			TimerUtil timerUtil = new TimerUtil(action, time, timerObj);
			timerObj.GetComponent<TimerMonoBehaviour>().onUpdate = timerUtil.UpdateTimer;
			return timerUtil;
		}
		
		private class TimerMonoBehaviour : MonoBehaviour
		{
			public Action onUpdate;
			private void Update ()
			{
				onUpdate();
			}
		}
		
		private Action _act;
		private float _time;
		private GameObject _timerObj;

		public bool isEnded { get; private set; }
		public bool isPaused { get; set; }
		public float TimeRemaining => _time;

		private TimerUtil (Action action, float time, GameObject timerObj)
		{
			_act = action;
			_time = time;
			_timerObj = timerObj;
			
			//Ensuring the bools are correctly initialized as false
			isEnded = false;
			isPaused = false;
		}

		private void UpdateTimer ()
		{
			if (isEnded || isPaused) return;
			
			_time -= Time.deltaTime;
			if (_time <= 0)
			{
				EndWithAction();
			}
		}

		/// <summary>
		/// Ends the timer and destroys associated GameObject. Cannot be undone
		/// </summary>
		public void EndTimer ()
		{
			isEnded = true;
			UnityEngine.Object.Destroy(_timerObj);
		}

		/// <summary>
		/// Ends the timer and invokes its action. Cannot be undone.
		/// </summary>
		public void EndWithAction ()
		{
			_act();
			EndTimer();
		}

		/// <summary>
		/// Adds specified time to the currently active timer
		/// </summary>
		public void AddTime (float time)
		{
			if (isEnded) return;
			
			_time += time;
		}

	}
}