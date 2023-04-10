using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boxfriend.Utils
{
	/// <summary>
	/// Random useful methods
	/// </summary>
	public class Utils
	{
		/// <summary>
		/// Determines if the supplied ints have opposite signs
		/// </summary>
		public static bool OppositeSigns (int x, int y)
		{
			return ((x ^ y) < 0);
		}
		
		/// <summary>
		/// Formats number of bytes to string
		/// </summary>
		public static string FormatBytes(long bytes)
		{
			string[] Suffix = {"B", "KB", "MB", "GB", "TB"};
			int i;
			double dblSByte = bytes;
			for (i = 0; i < Suffix.Length && bytes >= 1000; i++, bytes /= 1000)
			{
				dblSByte = bytes / 1000.0;
			}

			return $"{dblSByte:0.##} {Suffix[i]}";
		}
	}
}