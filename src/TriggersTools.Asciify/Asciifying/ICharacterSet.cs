using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Asciifying {
	public interface ICharacterSet {
		ParallelAbility ParallelAbility { get; }
		ICharacterEnumerator GetCharEnumerator(Point start, Size csize);
		string Characters { get; }
		string Name { get; }
		bool Vertical { get; }

		char[] ToArray();
	}

	public interface ICharacterEnumerator {

		IEnumerable<char> Available { get; }

		void Increment(char c);
		void NewLine();
	}

	public enum ParallelAbility {
		None,
		Line,
		Character,
		NoEnumerator,
	}
}
