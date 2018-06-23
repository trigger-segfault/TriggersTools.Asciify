using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using TriggersTools.Asciify.Asciifying.Palettes;
using TriggersTools.Asciify.Extensions;
using TriggersTools.Asciify.Utility;

namespace TriggersTools.Asciify.Asciifying.Fonts {
	public class BitmapFont {
		public Size Size { get; }
		public int Width => Size.Width;
		public int Height => Size.Height;
		public int Magnitude => Size.Width * Size.Height;

		public string Family { get; }

		public string FullName => $"{Family} {Width}x{Height}";

		public Bitmap Bitmap { get; }

		public BitmapFont(string family, Size size, Stream stream)
			: this(family, size, (Bitmap) Image.FromStream(stream))
		{
		}

		public BitmapFont(string family, Size size, Bitmap bitmap) {
			Family = family;
			Size = size;
			Bitmap = bitmap;
		}

		public int Columns => Bitmap.Width / Width;
		public int Rows => Bitmap.Height / Height;

		public Rectangle GetSourceRect(char c) {
			int column = c % Columns;
			int row = c / Columns;
			if (row >= Rows)
				throw new IndexOutOfRangeException();
			return new Rectangle(column * Width, row * Height, Width, Height);
		}

		public override string ToString() {
			return FullName;
		}
	}
	public class BitmapAsciifyFont : IAsciifyFont {

		private static Dictionary<string, Dictionary<Size, BitmapFont>> fonts;

		static BitmapAsciifyFont() {
			fonts = new Dictionary<string, Dictionary<Size, BitmapFont>>(
				StringComparer.OrdinalIgnoreCase);
			RegisterEmbeddedFontFamily("Terminal", new int[,] {
				{ 4, 6 },
				{ 6, 8 },
				{ 8, 8 },
				{ 16, 8 },
				{ 5, 12 },
				{ 7, 12 },
				{ 8, 12 },
				{ 16, 12 },
				{ 12, 16 },
				{ 10, 18 },
			});
		}

		public static void RegisterEmbeddedFontFamily(string family, int[,] sizes) {
			for (int i = 0; i < sizes.GetLength(0); i++) {
				RegisterEmbeddedFont(family, new Size(sizes[i, 0], sizes[i, 1]));
			}
		}
		
		public static BitmapFont RegisterEmbeddedFont(string family, Size size) {
			string path = Path.Combine(
				$"{nameof(TriggersTools)}.{nameof(Asciify)}.Resources.BitmapFonts." +
				$"{family}{size.Width}x{size.Height}.png");
			Stream stream = typeof(BitmapFont).Assembly.GetManifestResourceStream(path);
			return RegisterFont(family, size, stream);
		}

		public static BitmapFont RegisterFont(string family, Size size, string file) {
			using (FileStream stream = File.OpenRead(file))
				return RegisterFont(family, size, stream);
		}

		public static BitmapFont RegisterFont(string family, Size size, Stream stream) {
			if (!fonts.TryGetValue(family, out var sizes)) {
				sizes = new Dictionary<Size, BitmapFont>();
				fonts.Add(family, sizes);
			}
			BitmapFont font = new BitmapFont(family, size, stream);
			sizes.Add(size, font);
			return font;
		}

		public static BitmapFont GetFont(string family, Size size) {
			if (!fonts.TryGetValue(family, out var sizes))
				throw new Exception($"No font with the family name '{family}!");
			if (!sizes.TryGetValue(size, out var font))
				throw new Exception($"{family} family does not contain the size {size.Width}x{size.Height}!");
			return font;
		}

		public static IEnumerable<string> GetFamilies()
			=> fonts.Keys;

		public static IEnumerable<Size> GetSizes(string family) {
			if (!fonts.TryGetValue(family, out var sizes))
				throw new Exception($"No font with the family name '{family}!");
			return sizes.Keys;
		}

		public static IEnumerable<BitmapFont> GetFonts() {
			foreach (var sizes in fonts.Values) {
				foreach (BitmapFont font in sizes.Values)
					yield return font;
			}
		}

		public string Family => Font.Family;

		public BitmapFont Font { get; }
		public Size Size { get; }
		public int Width => Size.Width;
		public int Height => Size.Height;
		public int Magnitude => Size.Width * Size.Height;

		public ICharacterSet CharacterSet { get; }
		public IEnumerable<char> OutCharRange => CharacterSet.Characters;

		public BitmapAsciifyFont(string family, Size size, ICharacterSet ranges = null)
			: this(GetFont(family, size), ranges)
		{
		}

		public BitmapAsciifyFont(BitmapFont font, ICharacterSet ranges = null) {
			Font = font;
			Size = font.Size;
			if (ranges == null)
				ranges = CharacterSets.Bitmap;
			CharacterSet = ranges;
		}
		
		public void Setup(AsciifyPalette palette) {

		}

		public void Cleanup() {

		}
		
		public void DrawChar(Graphics g, char c, SolidBrush fore, SolidBrush back, Point point) {
			ColorMap[] mapping = new ColorMap[] {
				new ColorMap {
					OldColor = Color.Black,
					NewColor = back.Color,
				},
				new ColorMap {
					OldColor = Color.White,
					NewColor = fore.Color,
				},
			};
			ImageAttributes imageAttr = new ImageAttributes();
			imageAttr.SetRemapTable(mapping);

			Rectangle srcRect = Font.GetSourceRect(c);
			g.DrawImage(Font.Bitmap, point, srcRect, imageAttr);
		}

		public override string ToString() {
			return $"{Font}";
		}
	}
}
