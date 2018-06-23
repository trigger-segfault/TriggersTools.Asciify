using TriggersTools.Asciify.Asciifying.Palettes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace TriggersTools.Asciify.Asciifying.Fonts {
	public interface IAsciifyFont {

		ICharacterSet CharacterSet { get; }
		IEnumerable<char> OutCharRange { get; }
		Size Size { get; }
		int Width { get; }
		int Height { get; }
		int Magnitude { get; }

		void Setup(AsciifyPalette palette);
		void Cleanup();
		
		void DrawChar(Graphics g, char c, SolidBrush fore, SolidBrush back, Point point);
	}
}
