using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DevUtils.Elas.Tasks.Core.Reflection.Extensions;

namespace DevUtils.Elas.Tasks.Core.Extensions
{
	/// <summary> A string extensions. </summary>
	public static class StringExtensions
	{
		private static readonly MethodInfo GetCultureDataM;

		static StringExtensions()
		{
			var type = Type.GetType("System.Globalization.CultureData");

			if (type == null)
			{
				throw new NullReferenceException("Type \"System.Globalization.CultureData\" not found.");
			}

			GetCultureDataM = type.GetMethod("GetCultureData", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string), typeof(bool) }, null);

			if (GetCultureDataM == null)
			{
				throw new NullReferenceException("Method \"GetCultureData\" not found.");
			}
		}
		/// <summary> A String extension method that query if 'name' is valid culture name. </summary>
		///
		/// <param name="name"> The name to act on. </param>
		///
		/// <returns> true if valid culture name, false if not. </returns>
		public static bool IsValidCultureName(this String name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return false;
			}

			var culture = GetCultureDataM.InvokePreserveStackTrace(null, name, true);
			if (culture != null)
			{
				var ret = !Equals(culture, CultureInfo.InvariantCulture);
				return ret;
			}
			return false;
		}

		/// <summary> A string extension method that gets not randomized hash code. </summary>
		///
		/// <param name="text"> The text to act on. </param>
		///
		/// <returns> The not randomized hash code. </returns>
		public static int GetNotRandomizedHashCode(this string text)
		{
			unchecked
			{
				if (text == null)
				{
					return 0;
				}

				var elementComparer = EqualityComparer<char>.Default;

				var ret = text.Aggregate(17, (c, e) => c * 31 + elementComparer.GetHashCode(e));
				return ret;
			}
		}

		/// <summary> Equals ignore null or empty. </summary>
		///
		/// <param name="s1"> The first string. </param>
		/// <param name="s2"> The second string. </param>
		///
		/// <returns> true if equals ignore null or empty, false if not. </returns>
		public static bool EqualsIgnoreNullOrEmpty(string s1, string s2)
		{
			var noe = string.IsNullOrEmpty(s1);
			if (noe && string.IsNullOrEmpty(s2))
			{
				return true;
			}

			var ret = string.Equals(s1, s2);
			return ret;
		}
	}
}