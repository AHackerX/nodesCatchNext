using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace nodesCatchNext.Base;

internal static class StringEx
{
	public static bool IsNullOrEmpty(this string value)
	{
		return string.IsNullOrEmpty(value);
	}

	public static bool IsNullOrWhiteSpace(this string value)
	{
		return string.IsNullOrWhiteSpace(value);
	}

	public static bool BeginWithAny(this string s, IEnumerable<char> chars)
	{
		if (s.IsNullOrEmpty())
		{
			return false;
		}
		return chars.Contains(s[0]);
	}

	public static bool IsWhiteSpace(this string value)
	{
		for (int i = 0; i < value.Length; i++)
		{
			if (!char.IsWhiteSpace(value[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static IEnumerable<string> NonWhiteSpaceLines(this TextReader reader)
	{
		string text;
		while ((text = reader.ReadLine()) != null)
		{
			if (!text.IsWhiteSpace())
			{
				yield return text;
			}
		}
	}

	public static string TrimEx(this string value)
	{
		if (value != null)
		{
			return value.Trim();
		}
		return string.Empty;
	}
}
