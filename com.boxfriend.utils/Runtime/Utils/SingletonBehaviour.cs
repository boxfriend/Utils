using UnityEngine;

namespace Boxfriend.Utils
{
	public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
	{
		private static T _instance;
		[SerializeField] protected bool _dontDestroy;

		public static T Instance
		{
			get
			{
				if( _instance == null )
				{
					var go = new GameObject(typeof(T).Name);
					go.AddComponent<T>();
				}

				return _instance;
			}

			private set
			{
				if (_instance == null)
					_instance = value;
				else if (value != _instance)
					Destroy(value.gameObject);
			}
		}

		protected virtual void __internalAwake ()
		{
			Instance = (T)this;
            
			if(_dontDestroy)
				DontDestroyOnLoad(gameObject);
		}
	}
}
