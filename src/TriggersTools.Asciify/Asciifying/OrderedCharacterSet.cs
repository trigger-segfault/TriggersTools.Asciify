using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TriggersTools.Asciify.Extensions;

namespace TriggersTools.Asciify.Asciifying {
	public struct OrderedRules {
		public bool NoSpaces { get; set; }
		public bool ContinueOnNewLine { get; set; }
		public bool SkipOnSpace { get; set; }
		public bool Vertical { get; set; }
		public int InitialOffset { get; set; }

		public ParallelAbility ParallelAbility {
			get {
				if (SkipOnSpace || NoSpaces)
					return ParallelAbility.Character;
				else if (!ContinueOnNewLine)
					return ParallelAbility.Line;
				return ParallelAbility.None;
			}
		}
	}
	
	public class OrderedCharacterSet : ICharacterSet {

		public string Name { get; }
		public string Characters { get; }
		public string Text { get; }
		public OrderedRules Rules { get; }
		public ParallelAbility ParallelAbility { get; }
		public bool Vertical => Rules.Vertical;

		public OrderedCharacterSet(string text, OrderedRules rules, string name = null) {
			Name = name;
			string uniqueText = text.MakeUnique();
			if (rules.NoSpaces)
				Characters = uniqueText;
			else
				Characters = " " + uniqueText;
			Text = text;
			Rules = rules;
			ParallelAbility = rules.ParallelAbility;
			if (uniqueText.Contains(" "))
				throw new ArgumentException($"{nameof(text)} cannot contain spaces!");
		}

		public ICharacterEnumerator GetCharEnumerator(Point start, Size csize) {
			return new OrderedEnumerator(this, start, csize);
		}

		public char[] ToArray() => Characters.ToCharArray();
	}

	public class OrderedEnumerator : ICharacterEnumerator {
		public OrderedCharacterSet CharacterSet { get; }
		public int Index { get; private set; }

		public OrderedRules Rules => CharacterSet.Rules;
		public string Text => CharacterSet.Text;
		public ParallelAbility ParallelAbility => CharacterSet.ParallelAbility;


		internal OrderedEnumerator(OrderedCharacterSet charset, Point start, Size csize) {
			CharacterSet = charset;
			Index = Rules.InitialOffset;
			switch (ParallelAbility) {
			case ParallelAbility.None:
				if (start != Point.Empty) {
					throw new ArgumentException($"{nameof(start)} must be at (0,0) " +
						$"when {nameof(ParallelAbility)} is " +
						$"{nameof(ParallelAbility.None)}!");
				}
				break;
			case ParallelAbility.Line:
				if (start.X != 0 && !Rules.Vertical) {
					throw new ArgumentException($"{nameof(start)} must be at (0,y) " +
						$"when {nameof(ParallelAbility)} is " +
						$"{nameof(ParallelAbility.Line)} and not " +
						$"{nameof(Rules.Vertical)}!");
				}
				else if (start.Y != 0 && Rules.Vertical) {
					throw new ArgumentException($"{nameof(start)} must be at (x,0) " +
						$"when {nameof(ParallelAbility)} is " +
						$"{nameof(ParallelAbility.Line)} and " +
						$"{nameof(Rules.Vertical)}!");
				}
				break;
			}
			if (Rules.SkipOnSpace || Rules.NoSpaces) {
				if (Rules.ContinueOnNewLine) {
					if (Rules.Vertical)
						Index += start.X * csize.Height;
					else
						Index += start.Y * csize.Width;
				}
				if (Rules.Vertical)
					Index += start.Y;
				else
					Index += start.X;
			}
			Index %= Text.Length;
		}

		public IEnumerable<char> Available {
			get {
				if (!Rules.NoSpaces)
					yield return ' ';
				yield return CharacterSet.Text[Index];
			}
		}

		public void Increment(char c) {
			if (c != ' ' || Rules.SkipOnSpace || Rules.NoSpaces)
				Index = (Index + 1) % Text.Length;
		}

		public void NewLine() {
			if (!Rules.ContinueOnNewLine)
				Index = 0;
		}
	}
}
