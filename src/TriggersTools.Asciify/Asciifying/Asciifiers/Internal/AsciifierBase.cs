using TriggersTools.Asciify.Asciifying.Fonts;
using TriggersTools.Asciify.Asciifying.Palettes;
using TriggersTools.Asciify.ColorMine.Comparisons;
using TriggersTools.Asciify.ColorMine.Converters;
using TriggersTools.Asciify.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace TriggersTools.Asciify.Asciifying.Asciifiers {
	internal abstract class AsciifierBase<TFntData, TImgData> : IAsciifier {

		private static readonly Color Transparent = Color.FromArgb(0);
		private static readonly SolidBrush TransparentBrush = new SolidBrush(Transparent);

		protected ParallelOptions parallelOptions = new ParallelOptions {
			MaxDegreeOfParallelism = 1,
		};

		public IAsciifyFont Font { get; private set; }
		public AsciifyPalette Palette { get; private set; }
		public ICharacterSet CharacterSet { get; private set; }
		public int MaxDegreeOfParallelism {
			get => parallelOptions.MaxDegreeOfParallelism;
			set => parallelOptions.MaxDegreeOfParallelism = value;
		}

		public bool ForegroundOnly { get; set; }

		protected TFntData[] AllCharData { get; set; }
		protected MappedPixel[] AllMappedPixels { get; set; }
		protected Dictionary<char, int> AllCharIndexes { get; set; }
		
		protected int ColorCount { get; private set; }
		public IColorSpaceComparison Comparer { get; set; } = new Cie76Comparison();
		public Color ColorLow { get; set; } = Color.Black;
		public Color ColorHigh { get; set; } = Color.White;
		public Color ColorRange => Color.FromArgb(
			ColorHigh.A - ColorLow.A,
			ColorHigh.R - ColorLow.R,
			ColorHigh.G - ColorLow.G,
			ColorHigh.B - ColorLow.B);

		public ColorMatrix ColorMatrix {
			get {
				Color range = ColorRange;
				float rs = range.R / 255f;
				float rt = ColorLow.R / 255f;
				float gs = range.G / 255f;
				float gt = ColorLow.G / 255f;
				float bs = range.B / 255f;
				float bt = ColorLow.B / 255f;
				return new ColorMatrix(new float[][] {
					new float[] { rs,  0,  0, 0, 0 },
					new float[] {  0, gs,  0, 0, 0 },
					new float[] {  0,  0, bs, 0, 0 },
					new float[] {  0,  0,  0, 1, 0 },
					new float[] { rt, gt, bt, 0, 1 },
				});
			}
		}

		public AsciifierBase() {
		}

		public void Initialize(IAsciifyFont font, ICharacterSet charset, AsciifyPalette palette) {
			Font = font;
			Palette = palette;
			CharacterSet = charset;
			char[] charArray = CharacterSet.Characters.ToCharArray();
			PaletteColor[] colorArray = palette.ToArray();
			int charCount = charArray.Length;
			int colorCount = colorArray.Length;
			ColorCount = colorCount;
			int count = charCount * colorCount;
			int index = 0;
			Stopwatch watch = Stopwatch.StartNew();
			AllCharData = new TFntData[count];
			AllMappedPixels = new MappedPixel[count];
			Report("Array Creation", watch);
			AllCharIndexes = new Dictionary<char, int>(charCount);
			PreInitialize();
			using (Bitmap bitmap = new Bitmap(font.Width, font.Height, PixelFormat.Format24bppRgb))
			using (PaletteBrushes brushes = Palette.CreateBrushes()) {
				Font.Setup(Palette);
				try {
					for (int i = 0; i < charCount; i++) {
						char c = charArray[i];
						AllCharIndexes.Add(c, index);
						for (int j = 0; j < colorCount; j++) {
							PaletteColor color = colorArray[j];
							AllMappedPixels[index] = new MappedPixel(c, i, color, index);
							AllCharData[index] = CalcCharData(bitmap, brushes, c, i, color);
							index++;
						}
					}
				}
				finally {
					Font.Cleanup();
				}
			}
			PostInitialize();
		}

		protected virtual void PreInitialize() { }

		protected virtual void PostInitialize() { }

		protected abstract double CalcScore(TImgData a, TFntData b);

		protected abstract TFntData CalcFontData(Color color);

		protected abstract TFntData CalcFontData(IEnumerable<PixelPoint> pixels);

		protected abstract TImgData CalcImageData(IEnumerable<PixelPoint> pixels, Point start);


		protected IEnumerable<PixelPoint> ForEachCharPixel(BitmapData data, Point start) {
			Size minSize = new Size(
				Math.Min(Font.Width, data.Width - start.X),
				Math.Min(Font.Height, data.Height - start.Y));
			for (int y = 0; y < minSize.Height; y++) {
				for (int x = 0; x < minSize.Width; x++) {
					yield return new PixelPoint(x, y, data.GetColor(start.X + x, start.Y + y));
				}
			}
		}

		protected IEnumerable<Point> ForEachChar(Size csize) {
			Size fsize = Font.Size;
			csize.Width *= fsize.Width;
			csize.Height *= fsize.Height;
			for (int y = 0; y < csize.Height; y += fsize.Height) {
				for (int x = 0; x < csize.Width; x += fsize.Width) {
					yield return new Point(x, y);
				}
			}
		}

		protected IEnumerable<Point> ForEachChar(Size csize, bool vertical) {
			Size fsize = Font.Size;
			csize.Width *= fsize.Width;
			csize.Height *= fsize.Height;
			if (vertical) {
				for (int x = 0; x < csize.Width; x += fsize.Width) {
					for (int y = 0; y < csize.Height; y += fsize.Height) {
						yield return new Point(x, y);
					}
				}
			}
			else {
				for (int y = 0; y < csize.Height; y += fsize.Height) {
					for (int x = 0; x < csize.Width; x += fsize.Width) {
						yield return new Point(x, y);
					}
				}
			}
		}

		protected IEnumerable<Point> ForEachLine(Size csize, bool vertical) {
			if (vertical) {
				for (int x = 0; x < csize.Width; x++)
					yield return new Point(x, 0);
			}
			else {
				for (int y = 0; y < csize.Height; y++)
					yield return new Point(0, y);
			}
		}

		protected IEnumerable<Point> ForEachInLine(Point cstart, Size csize, bool vertical) {
			Size fsize = Font.Size;
			if (vertical) {
				cstart.X *= fsize.Width;
				csize.Height *= fsize.Height;
				for (int y = 0; y < csize.Height; y += fsize.Height)
					yield return new Point(cstart.X, y);
			}
			else {
				cstart.Y *= fsize.Height;
				csize.Width *= fsize.Width;
				for (int x = 0; x < csize.Width; x += fsize.Width)
					yield return new Point(x, cstart.Y);
			}
		}

		private TFntData CalcCharData(Bitmap bitmap, PaletteBrushes brushes, char c, int index, PaletteColor color) {
			if (ForegroundOnly) {
				if (c == ' ')
					return CalcFontData(color.ColorB);
				else
					return CalcFontData(color.ColorF);
			}
			using (Graphics g = CreateGraphics(bitmap)) {
				SolidBrush fore = brushes[color.IndexF, true];
				SolidBrush back = brushes[color.IndexB, false];
				Font.DrawChar(g, c, fore, back, Point.Empty);
			}
			using (DisposableBitmapData bmpData = bitmap.LockRead()) {
				return CalcFontData(bmpData.Data.ForEachPixel());
			}
		}

		private Graphics CreateGraphics(Bitmap bitmap) {
			Graphics g = Graphics.FromImage(bitmap);
			g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			return g;
		}

		protected virtual MappedPixel FindBestPixel(BitmapData data, Point start,
			ICharacterEnumerator enumerator)
		{
			TImgData charData = CalcImageData(ForEachCharPixel(data, start), start);
			double bestScore = float.MaxValue;
			int bestPixelIndex = 0;
			//int count = AllMappedPixels.Length;
			foreach (int i in ForEachMappedPixel(enumerator)) {
			//for (int i = 0; i < count; i++) {
				double score = CalcScore(charData, AllCharData[i]);
				if (score < bestScore) {
					bestScore = score;
					bestPixelIndex = i;
					if (score == 0)
						break;
				}
			}
			return AllMappedPixels[bestPixelIndex];
		}

		protected IEnumerable<int> ForEachMappedPixel() {
			int count = AllMappedPixels.Length;
			for (int i = 0; i < count; i++)
				yield return i;
		}
		protected IEnumerable<int> ForEachMappedPixel(ICharacterEnumerator enumerator) {
			if (enumerator == null) {
				int count = AllMappedPixels.Length;
				for (int i = 0; i < count; i++)
					yield return i;
			}
			else {
				foreach (char c in enumerator.Available) {
					int start = AllCharIndexes[c];
					int end = start + ColorCount;
					for (int i = start; i < end; i++)
						yield return i;
				}
			}
		}
		
		public Bitmap AsciifyImage(Bitmap input) {
			Stopwatch totalWatch = Stopwatch.StartNew();
			using (DisposableBitmapData bmpData = input.LockRead())
			using (PaletteBrushes brushes = Palette.CreateBrushes()) {
				Font.Setup(Palette);
				try {
					Size csize = new Size(
						(int) Math.Ceiling((double) input.Width / Font.Width),
						(int) Math.Ceiling((double) input.Height / Font.Height));
					Bitmap output = new Bitmap(csize.Width * Font.Width, csize.Height * Font.Height, PixelFormat.Format24bppRgb);
					try {
						using (Graphics g = CreateGraphics(output))
							AsciifyGraphics(g, bmpData, output, csize, brushes);
						Report("Time", totalWatch);
						return output;
					}
					catch {
						output.Dispose();
						throw;
					}
				}
				finally {
					Font.Cleanup();
				}
			}
		}

		private void FindAndDrawChar(Graphics g, BitmapData data, Point point,
			PaletteBrushes brushes, ICharacterEnumerator enumerator)
		{
			MappedPixel pixel = FindBestPixel(data, point, enumerator);
			SolidBrush fore = brushes[pixel.IndexF, true];
			SolidBrush back = brushes[pixel.IndexB, false];
			lock (g) {
				Font.DrawChar(g, pixel.Char, fore, back, point);
			}
			enumerator?.Increment(pixel.Char);
		}

		private void AsciifyGraphics(Graphics g, BitmapData data, Bitmap output, Size csize, PaletteBrushes brushes) {
			int cmag = csize.Width * csize.Height;
			int index = 0;
			ParallelAbility ability = CharacterSet.ParallelAbility;
			if (MaxDegreeOfParallelism == 1)
				ability = ParallelAbility.None;
			switch (CharacterSet.ParallelAbility) {
			case ParallelAbility.NoEnumerator:
				Parallel.ForEach(ForEachChar(csize, CharacterSet.Vertical),
					parallelOptions, p =>
				{
					if (p.X == 0)
						Console.WriteLine($"{index}/{cmag}");
					FindAndDrawChar(g, data, p, brushes, null);
					index++;
				});
				break;
			case ParallelAbility.Line:
				Parallel.ForEach(ForEachLine(csize, CharacterSet.Vertical),
					parallelOptions, cstart =>
				{
					ICharacterEnumerator enumerator = CharacterSet.GetCharEnumerator(cstart, csize);
					Console.WriteLine($"{index}/{cmag}");
					foreach (Point p in ForEachInLine(cstart, csize, CharacterSet.Vertical)) {
						FindAndDrawChar(g, data, p, brushes, enumerator);
						index++;
					}
				});
				break;
			case ParallelAbility.Character:
				Parallel.ForEach(ForEachChar(csize, CharacterSet.Vertical),
					parallelOptions, p =>
				{
					if (p.X == 0)
						Console.WriteLine($"{index}/{cmag}");
					Point cstart = new Point(p.X / Font.Width, p.Y / Font.Height);
					ICharacterEnumerator enumerator = CharacterSet.GetCharEnumerator(cstart, csize);
					FindAndDrawChar(g, data, p, brushes, enumerator);
					index++;
				});
				break;
			case ParallelAbility.None:
				ICharacterEnumerator enumerator2 = CharacterSet.GetCharEnumerator(Point.Empty, csize);
				foreach (Point p in ForEachChar(csize, CharacterSet.Vertical)) {
					FindAndDrawChar(g, data, p, brushes, enumerator2);
					index++;
					if ((!CharacterSet.Vertical && p.X == output.Width - 1) ||
						(CharacterSet.Vertical && p.Y == output.Height - 1)) {
						enumerator2?.NewLine();
						Console.WriteLine($"{index}/{cmag}");
					}
				}
				break;
			}
			Console.WriteLine($"{index}/{cmag}");
		}

		private void Report(string name, Stopwatch watch) {
			Trace.WriteLine($"{name}: {watch.ElapsedMilliseconds}ms");
		}

		public Bitmap PrepareImage(Image image, double scale, Color transparent) {
			if (scale * image.Width < 1 || scale * image.Height < 1)
				throw new ArgumentException("Scale too small!");
			
			Size scaled = new Size(
				(int) (image.Width * scale),
				(int) (image.Height * scale));
			Bitmap bitmap = new Bitmap(scaled.Width, scaled.Height, PixelFormat.Format24bppRgb);
			try {
				using (Graphics g = Graphics.FromImage(bitmap)) {
					g.InterpolationMode = InterpolationMode.High;
					g.CompositingQuality = CompositingQuality.HighQuality;
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.Clear(transparent);

					ImageAttributes attributes = new ImageAttributes();
					attributes.SetColorMatrix(ColorMatrix);

					PrepareImage(g, image, scaled, transparent, attributes);
					return bitmap;
				}
			}
			catch {
				bitmap.Dispose();
				throw;
			}
		}

		protected virtual void PrepareImage(Graphics g, Image image, Size scaled, Color transparent, ImageAttributes attributes) {
			DrawImage(g, image, scaled, attributes);
		}

		protected void DrawImage(Graphics g, Image image, Size scaled, ImageAttributes attributes) {
			g.DrawImage(image, new Rectangle(0, 0, scaled.Width, scaled.Height),
				0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
		}

		public static ColorMatrix MultiplyMatrix(ColorMatrix a, ColorMatrix b) {
			float[][] c = new float[5][];
			for (int d = 0; d < 5; d++)
				c[d] = new float[5];
			for (int i = 0; i < 5; i++) {
				for (int j = 0; j < 5; j++) {
					for (int k = 0; k < 5; k++) // OR k<b.GetLength(0)
						c[i][j] += a[i, k] * b[k, j];
				}
			}
			return new ColorMatrix(c);
		}
	}
}
