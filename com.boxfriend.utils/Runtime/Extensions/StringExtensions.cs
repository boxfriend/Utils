using System.Runtime.CompilerServices;
using UnityEngine;
namespace Boxfriend.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Checks if two strings are the same without case sensitivity.
		/// </summary>
		/// <param name="value">String being compared</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CaseInsensitveEquals (this string str, string value) => (str.ToLower() == value.ToLower());
		
		/// <summary>
		/// Applies a rich text color to string
		/// </summary>
		/// <param name="text">String to be colored</param>
		/// <param name="col">Unity Color applied to all of 'text'</param>
		public static string AddColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>"; 
		public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";
	}
}
