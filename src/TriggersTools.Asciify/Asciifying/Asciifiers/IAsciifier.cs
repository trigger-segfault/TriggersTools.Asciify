using TriggersTools.Asciify;
using TriggersTools.Asciify.Asciifying;
using TriggersTools.Asciify.Asciifying.Fonts;
using TriggersTools.Asciify.Asciifying.Palettes;
using TriggersTools.Asciify.ColorMine.Comparisons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	public interface IAsciifier {

		IAsciifyFont Font { get; }
		AsciifyPalette Palette { get; }
		ICharacterSet CharacterSet { get; }

		Color ColorLow { get; set; }
		Color ColorHigh { get; set; }
		//IColorSpaceComparison Comparer { get; set; }
		//double MaxIntensityScale { get; set; }

		void Initialize(IAsciifyFont font, ICharacterSet charset, AsciifyPalette palette);

		Bitmap AsciifyImage(Bitmap preparedImage);

		Bitmap PrepareImage(Image image, double scale, Color transparent);

		int MaxDegreeOfParallelism { get; set; }

		bool ForegroundOnly { get; set; }
	}
	public interface IColorAsciifier : IAsciifier {
		IColorSpaceComparison Comparer { get; set; }

		//bool IndividualLabs { get; set; }
	}
	public interface IIntensityAsciifier : IAsciifier {
		// Intensity
		//bool ReverseIntensity { get; set; }
	}
}
