using System.Runtime.CompilerServices;
using UnityEngine;
namespace Boxfriend.Extensions
{
	public static class MathExtensions
	{
		/// <summary>
		/// Checks if value is within specified range.
		/// </summary>
		/// <param name="min">Lowest value of the range</param>
		/// <param name="max">Largest value of the range</param>
		/// <returns>True if less than min and greater than max</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InRange(this int value, int min, int max) => (value >= min) && (value <= max);
		/// <summary>
		/// Checks if value is within specified range.
		/// </summary>
		/// <param name="min">Lowest value of the range</param>
		/// <param name="max">Largest value of the range</param>
		/// <returns>True if less than min and greater than max</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InRange(this float value, float min, float max) => (value >= min) && (value <= max);
		/// <summary>
		/// Checks if value is within specified range.
		/// </summary>
		/// <param name="min">Lowest value of the range</param>
		/// <param name="max">Largest value of the range</param>
		/// <returns>True if less than min and greater than max</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool InRange(this double value, double min, double max) => (value >= min) && (value <= max);
		
		public static Vector2 Rotate(this Vector2 vector, float degrees)
		{
			float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
			float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

			float tx = vector.x;
			float ty = vector.y;
			vector.x = (cos * tx) - (sin * ty);
			vector.y = (sin * tx) + (cos * ty);
			return vector;
		}
	}
}
