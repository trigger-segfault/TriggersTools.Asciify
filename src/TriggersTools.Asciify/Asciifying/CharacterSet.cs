using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Asciifying {
	public static class CharacterSets {

		public static readonly CharacterSet Default = CharacterSetBuilder.FromRange(32, 127, "Default");
		public static readonly CharacterSet Digits = CharacterSetBuilder.FromRange('0', '9', "Digits");
		public static readonly CharacterSet Letters = CharacterSetBuilder.FromRange('A', 'Z', 'a', 'z', "Letters");
		public static readonly CharacterSet Bitmap = CharacterSetBuilder.FromRange(1, 254, "Bitmap");
		public static readonly CharacterSet Unicode = CharacterSetBuilder.FromRange(32, 127, 161, 255, "Unicode");
		public static readonly CharacterSet UnicodeFull = CharacterSetBuilder.FromRange(32, 0x2A0, "Unicode Full");

	}

	public class CharacterSet : IEnumerable<char>, ICharacterSet {
		
		public string Name { get; }
		public string Characters { get; }

		internal CharacterSet(string characters, string name = null) {
			Name = name;
			Characters = characters;
		}

		public static implicit operator CharacterSet(string str) {
			return CharacterSetBuilder.FromString(str);
		}

		public ParallelAbility ParallelAbility => ParallelAbility.NoEnumerator;
		public bool Vertical => false;

		public override string ToString() {
			return $"{(Name ?? "Unnamed")} Charset: {Characters.Length}";
		}

		public int Count => Characters.Length;

		public IEnumerator<char> GetEnumerator() {
			return Characters.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Characters.GetEnumerator();
		}

		public ICharacterEnumerator GetCharEnumerator(Point start, Size csize) {
			return null;
		}

		public char[] ToArray() => Characters.ToCharArray();
	}

	public class CharacterSetBuilder : HashSet<char> {

		public CharacterSetBuilder() { }

		public CharacterSetBuilder(char single) {
			Add(single);
		}

		public CharacterSetBuilder(int single) {
			Add(single);
		}

		public CharacterSetBuilder(char start, char end) {
			Add(start, end);
		}

		public CharacterSetBuilder(int start, int end) {
			Add(start, end);
		}

		public CharacterSetBuilder(string str) {
			Add(str);
		}

		public CharacterSetBuilder(IEnumerable<char> chars) {
			Add(chars);
		}


		public new bool Add(char single) {
			return base.Add(single);
		}

		public bool Add(int single) {
			return base.Add((char) single);
		}

		public int Add(char start, char end) {
			if (end < start)
				throw new ArgumentException("End cannot be less than start!", nameof(end));
			int count = 0;
			for (char c = start; c <= end; c++) {
				if (Add(c))
					count++;
			}
			return count;
		}

		public int Add(int start, int end) {
			return Add((char) start, (char) end);
		}

		public int Add(string str) {
			int count = 0;
			foreach (char c in str) {
				if (Add(c))
					count++;
			}
			return count;
		}

		public int Add(IEnumerable<char> chars) {
			int count = 0;
			foreach (char c in chars) {
				if (Add(c))
					count++;
			}
			return count;
		}

		public override string ToString() => string.Join("", this);
		
		public CharacterSet Build(string name = null) =>
			new CharacterSet(string.Join("", this), name);

		public static CharacterSet FromSingle(char single, string name = null) {
			return new CharacterSet(new string(single, 1), name);
		}

		public static CharacterSet FromSingle(int single, string name = null) {
			return FromSingle((char) single, name);
		}

		public static CharacterSet FromRange(char start, char end, string name = null) {
			return new CharacterSetBuilder() {
				{ start, end },
			}.Build(name);
		}

		public static CharacterSet FromRange(int start, int end, string name = null) {
			return FromRange((char) start, (char) end, name);
		}

		public static CharacterSet FromRange(char start1, char end1, char start2, char end2, string name = null) {
			return new CharacterSetBuilder() {
				{ start1, end1 },
				{ start2, end2 },
			}.Build(name);
		}

		public static CharacterSet FromRange(int start1, int end1, int start2, int end2, string name = null) {
			return FromRange((char) start1, (char) end1, (char) start2, (char) end2, name);
		}

		public static CharacterSet FromString(string str, string name = null) {
			return new CharacterSetBuilder() {
				{ str },
			}.Build(name);
		}

		public static CharacterSet FromChars(IEnumerable<char> chars, string name = null) {
			return new CharacterSetBuilder() {
				{ chars },
			}.Build(name);
		}
	}
}
