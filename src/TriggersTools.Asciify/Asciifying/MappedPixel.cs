using TriggersTools.Asciify.Asciifying.Palettes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace TriggersTools.Asciify.Asciifying {
	public struct MappedPixel {
		public MappedPixel(char c, int indexCh, PaletteColor color, int index) {
			Char = c;
			IndexCh = indexCh;
			ColorF = color.ColorF;
			ColorB = color.ColorB;
			IndexF = color.IndexF;
			IndexB = color.IndexB;
			Index = index;
		}

		public char Char;
		public Color ColorF;
		public Color ColorB;
		public int IndexCh;
		public int IndexF;
		public int IndexB;
		public int Index;
	}
}
