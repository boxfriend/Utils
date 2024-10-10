using UnityEngine;

namespace Boxfriend.Extensions
{
	public static class TransformExtensions
	{
		public static T GetComponentInInactiveParent<T>(this Transform transform) where T : Component
		{
			while(transform.parent != null)
			{
				transform = transform.parent;
				if(transform.TryGetComponent(out T comp))
					return comp;
			}
			return null;
		}
		
		public static void UnparentAll(this Transform transform)
		{
			foreach(Transform child in transform)
			{
				child.UnparentAll();
			}
			transform.SetParent(null);
		}
	}
}
