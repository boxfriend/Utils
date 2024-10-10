using UnityEngine;
namespace Boxfriend.Extensions
{
	public static class Vector2Extensions
	{
		/// <summary>
		/// Changes a Vector2 into a Vector3 where the V2 Y axis is represented on the V3 Z axis
		/// </summary>
		public static Vector3 To3D(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);
	}
}
