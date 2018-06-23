using System;
using System.Collections.Generic;
using System.Text;

namespace TriggersTools.Asciify.Extensions {
	public static class StringExtensions {

		public static string MakeUnique(this string str) {
			HashSet<char> characters = new HashSet<char>();
			foreach (char c in str)
				characters.Add(c);
			return string.Join("", characters);
		}
	}
}
