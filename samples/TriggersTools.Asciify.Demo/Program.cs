using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TriggersTools.Asciify.Asciifying;
using TriggersTools.Asciify.Asciifying.Asciifiers;
using TriggersTools.Asciify.Asciifying.Fonts;
using TriggersTools.Asciify.Asciifying.Palettes;
using TriggersTools.Asciify.Extensions;

namespace TriggersTools.Asciify.Demo {
	class Program {
		static Color Gray(int value) {
			return Color.FromArgb(value, value, value);
		}

		const int MaxCores = 3;

		static readonly Color DiscordDark = Color.FromArgb(54, 57, 62);

		static void Main(string[] args) {
			/*SmugAnimeFace();
			StandardLink();
			MakinaRoger();*/
			Project64Console();
			/*PsychoPass();
			PsychoPassGrayscale();
			EyeBlackAndWhite();*/
			return;
		}

		static Stopwatch ReportTimer = new Stopwatch();

		static void ReportStart([CallerMemberName]string name = null) {
			Console.WriteLine($"{name} Start:");
			ReportTimer.Restart();
		}
		static void ReportEnd([CallerMemberName]string name = null) {
			Console.WriteLine($"{name} Finished: {ReportTimer.ElapsedMilliseconds}ms");
			ReportTimer.Stop();
		}

		static void SmugAnimeFace() {
			// Source: https://www.reddit.com/r/Nisekoi/comments/49qsnx/made_a_minimalist_wallpaper_for_smug_marika/
			ReportStart();
			string url = @"https://i.imgur.com/Ne42TUC.png";
			using (Bitmap input = DownloadImage(url)) {
				IAsciifier asciifier = Asciifier.SectionedColor;
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = new OrderedCharacterSet("SMUGANIMEFACE", new OrderedRules());
				IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);

				// Color
				asciifier.ForegroundOnly = true;
				Color background = Color.FromArgb(113, 113, 113);
				List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 5);
				AsciifyPalette palette = new AsciifyPalette(colors, background: background);

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 1, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static void StandardLink() {
			// Source: https://hyruleanassassin.deviantart.com/art/Link-Minimalist-Wallpaper-647717661
			ReportStart();
			string url = @"https://pre00.deviantart.net/254c/th/pre/i/2017/172/5/8/link_minimalist_wallpaper_by_hyruleanassassin-dapmu59.jpg";
			using (Bitmap input = DownloadImage(url)) {
				IAsciifier asciifier = Asciifier.SectionedColor;
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = new OrderedCharacterSet("COURAGE", new OrderedRules());
				IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);

				// Color
				asciifier.ForegroundOnly = true;
				Color background = Color.FromArgb(2, 38, 2);
				List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 8);
				AsciifyPalette palette = new AsciifyPalette(colors, background: background);

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 1, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static void MakinaRoger() {
			// Source: Grisaia no Kajitsu
			ReportStart();
			string url = @"https://i.imgur.com/KDxsmOd.png";
			using (Bitmap input = DownloadImage(url)) {
				IAsciifier asciifier = Asciifier.SectionedColor;
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = CharacterSets.Default;
				IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);

				// Color
				Color background = DiscordDark;
				List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 7);
				AsciifyPalette palette = new AsciifyPalette(colors, background: background);

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 1, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static void Project64Console() {
			// Source: https://www.pokemonemulators.com/project64/
			ReportStart();
			string url = @"https://i.imgur.com/zzCG56R.png";
			using (Bitmap input = DownloadImage(url)) {
				IAsciifier asciifier = Asciifier.SectionedColor;
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = CharacterSets.Bitmap;
				IAsciifyFont font = new BitmapAsciifyFont("Terminal", new Size(8, 12), charset);

				// Color
				Color background = Color.Black;
				AsciifyPalette palette = AsciifyPalette.WindowsConsole;

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 2, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static void PsychoPass() {
			// Source: http://www.nerdgasmneeds.com/2016/02/new-psycho-pass-dominator-replica-video.html
			ReportStart();
			string url = @"http://www.nerdgasmneeds.com/wp/wp-content/uploads/2016/02/psycho-pass-dominator-art-03.png";
			using (Bitmap input = DownloadImage(url)) {
				ISectionedAsciifier asciifier = Asciifier.SectionedColor;
				//asciifier.AllFactor
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				//ICharacterSet charset = CharacterSets.Default;
				//IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);
				ICharacterSet charset = CharacterSets.Bitmap;
				IAsciifyFont font = new BitmapAsciifyFont("Terminal", new Size(8, 12), charset);

				// Color
				//asciifier.ColorLow = Gray(50);
				//asciifier.ColorHigh = Gray(220);
				Color background = DiscordDark;
				//List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 1.5);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				//AsciifyPalette palette = new AsciifyPalette(colors, background: background);
				List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 16, 14);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				AsciifyPalette palette = new AsciifyPalette(colors);

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 1.5, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static void PsychoPassGrayscale() {
			// Source: http://www.nerdgasmneeds.com/2016/02/new-psycho-pass-dominator-replica-video.html
			ReportStart();
			string url = @"http://www.nerdgasmneeds.com/wp/wp-content/uploads/2016/02/psycho-pass-dominator-art-03.png";
			using (Bitmap input = DownloadImage(url, nameof(PsychoPass))) {
				ISectionedAsciifier asciifier = Asciifier.SectionedIntensity;
				//asciifier.AllFactor
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = CharacterSets.Default;
				IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);
				//ICharacterSet charset = CharacterSets.Bitmap;
				//IAsciifyFont font = new BitmapAsciifyFont("Terminal", new Size(8, 12), charset);

				// Color
				asciifier.ColorLow = Gray(10);
				asciifier.ColorHigh = Gray(230);
				Color background = Color.Black;
				//List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 1.5);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				//AsciifyPalette palette = new AsciifyPalette(colors, background: background);
				//List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 16, 14);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				AsciifyPalette palette = AsciifyPalette.FromGrayscale(0, 255, 16, background: background);

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 1.5, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}


		static void EyeBlackAndWhite() {
			// Source: https://mathematica.stackexchange.com/questions/42638/creating-an-image-from-data-not-in-grayscale
			ReportStart();
			string url = @"http://i.stack.imgur.com/397vv.png";
			using (Bitmap input = DownloadImage(url)) {
				IAsciifier asciifier = Asciifier.DotIntensity;
				//asciifier.AllFactor
				asciifier.MaxDegreeOfParallelism = MaxCores;

				// Text
				ICharacterSet charset = CharacterSets.Default;
				IAsciifyFont font = new TrueTypeAsciifyFont("Lucida Console", 10, FontStyle.Bold, charset, true);

				// Color
				asciifier.ColorLow = Gray(130);
				//asciifier.ColorHigh = Gray(220);
				Color background = Color.Gray;
				//List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 256, 1.5);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				//AsciifyPalette palette = new AsciifyPalette(colors, background: background);
				//List<Color> colors = PaletteChooser.FindMainColorsByLab(input, 16, 14);
				//colors = PaletteChooser.FindMainColorsByHsb(input, 256, 0.1f, 0.001f, 0.001f);
				AsciifyPalette palette = AsciifyPalette.BlackOnWhite;

				// Asciify
				asciifier.Initialize(font, charset, palette);
				using (Bitmap prepared = asciifier.PrepareImage(input, 3, background))
				using (Bitmap output = asciifier.AsciifyImage(prepared))
					output.OpenInMSPaint();
			}
			ReportEnd();
		}

		static Bitmap DownloadImage(string url, [CallerMemberName] string fileName = null) {
			string file = Path.ChangeExtension(Path.Combine("Downloaded", fileName), ".png");
			if (File.Exists(file))
				return (Bitmap) Image.FromFile(file);
			Directory.CreateDirectory("Downloaded");
			using (HttpClient client = new HttpClient()) {
				var task = client.GetStreamAsync(url);
				task.Wait();
				using (Stream stream = task.Result) {
					Bitmap bitmap = (Bitmap) Image.FromStream(stream);
					bitmap.Save(file);
					return bitmap;
				}
			}
		}
	}
}
