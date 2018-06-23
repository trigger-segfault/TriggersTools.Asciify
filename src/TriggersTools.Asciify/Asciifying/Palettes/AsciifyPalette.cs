using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace TriggersTools.Asciify.Asciifying.Palettes {
	public struct PaletteColor {
		public Color ColorF { get; set; }
		public Color ColorB { get; set; }
		public int IndexF { get; set; }
		public int IndexB { get; set; }
		public int LookupF => Math.Max(0, IndexF);
		public int LookupB => Math.Max(0, IndexB);

		public PaletteColor(Color fore, Color back, int indexF = -1, int indexB = -1) {
			ColorF = fore;
			ColorB = back;
			IndexF = indexF;
			IndexB = indexB;
		}
	}
	public class AsciifyPalette : IEnumerable<PaletteColor> {

		public static readonly AsciifyPalette WindowsConsole;
		public static readonly AsciifyPalette BlackBackground;
		public static readonly AsciifyPalette WhiteBackground;
		public static readonly AsciifyPalette BlackOnWhite;
		public static readonly AsciifyPalette WhiteOnBlack;

		static AsciifyPalette() {
			WindowsConsole = new AsciifyPalette(
				new Color[16] {
					Color.FromArgb(  0,   0,   0),
					Color.FromArgb(  0,   0, 128),
					Color.FromArgb(  0, 128,   0),
					Color.FromArgb(  0, 128, 128),
					Color.FromArgb(128,   0,   0),
					Color.FromArgb(128,   0, 128),
					Color.FromArgb(128, 128,   0),
					Color.FromArgb(192, 192, 192),
					Color.FromArgb(128, 128, 128),
					Color.FromArgb(  0,   0, 255),
					Color.FromArgb(  0, 255,   0),
					Color.FromArgb(  0, 255, 255),
					Color.FromArgb(255,   0,   0),
					Color.FromArgb(255,   0, 255),
					Color.FromArgb(255, 255,   0),
					Color.FromArgb(255, 255, 255),
				}
			);
			BlackBackground = new AsciifyPalette(
				WindowsConsole.Colors,
				background: Color.Black
			);
			WhiteBackground = new AsciifyPalette(
				WindowsConsole.Colors,
				background: Color.White
			);
			BlackOnWhite = new AsciifyPalette(Color.Black, Color.White);
			WhiteOnBlack = new AsciifyPalette(Color.White, Color.Black);
		}

		private static Color Gray(int value) => Color.FromArgb(value, value, value);

		public static AsciifyPalette FromGrayscale(int min, int max, int count,
			Color? foreground = null, Color? background = null)
		{
			if (count < 1)
				throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be less than one!");
			if (min > max)
				throw new ArgumentOutOfRangeException(nameof(min), "Min cannot be greater than max!");
			count = Math.Min(max - min + 1, count);
			int range = max - min;

			Color[] colors = new Color[count];
			colors[0] = Gray(min);
			for (int i = 0; i < count; i++) {
				colors[i] = Gray(min + i * range / (count - 1));
			}

			return new AsciifyPalette(colors, foreground, background);
		}
		
		public AsciifyPalette(IEnumerable<Color> colors, Color? foreground = null, Color? background = null) {
			Colors = colors.ToList().AsReadOnly();
			Foreground = foreground;
			Background = background;
		}

		public AsciifyPalette(Color? foreground = null, Color? background = null) {
			Colors = new List<Color>().AsReadOnly();
			Foreground = foreground;
			Background = background;
		}

		public IReadOnlyList<Color> Colors { get; }

		public Color? Foreground { get; }
		public Color? Background { get; }

		public int Count => Colors.Count;

		public int CountF => Foreground.HasValue ? 1 : Count;
		public int CountB => Background.HasValue ? 1 : Count;
		public int CountFB => CountF * CountB;

		public IEnumerator<PaletteColor> GetEnumerator() {
			if (Foreground.HasValue) {
				if (Background.HasValue) {
					yield return new PaletteColor(Foreground.Value, Background.Value, -1, -1);
				}
				else {
					for (int b = 0; b < Count; b++) {
						if (Foreground.Value == Colors[b])
							continue;
						yield return new PaletteColor(Foreground.Value, Colors[b], -1, b);
					}
				}
			}
			else if (Background.HasValue) {
				for (int f = 0; f < Count; f++) {
					if (Background.Value == Colors[f])
						continue;
					yield return new PaletteColor(Colors[f], Background.Value, f, -1);
				}
			}
			else {
				for (int f = 0; f < Count; f++) {
					for (int b = 0; b < Count; b++) {
						if (Colors[f] == Colors[b])
							continue;
						yield return new PaletteColor(Colors[f], Colors[b], f, b);
					}
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public PaletteBrushes CreateBrushes() => new PaletteBrushes(Colors, Foreground, Background);
	}
}
