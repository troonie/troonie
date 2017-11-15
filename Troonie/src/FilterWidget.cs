using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Gtk;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using IOPath = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageConverter = Troonie_Lib.ImageConverter;
using Troonie;
using Troonie_Lib;
using System.Diagnostics;

namespace Troonie
{
	public delegate void FilterEventhandler(Bitmap filterBitmap);

	public partial class FilterWidget : Gtk.Window
	{
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private bool repeatTimeout, keepFilterImageInRam;
		private bool useSameImageSizeLikePreview;
		private int imageW; 
		private int imageH;
//		private string tempFilterImageFileName;
		private Bitmap image, filterImage;
		private GLib.TimeoutHandler timeoutHandler;
		private double[] filterProperties;
		private AbstractFilter abstractFilter;

		public FilterEventhandler FilterEvent;
		public string FileName { get; set; }

		#region Constructors

		protected FilterWidget (string pFilename) : base (Gtk.WindowType.Toplevel)
		{
			FileName = pFilename;
			KeepAbove = true;
			filterProperties = new double[8];
			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

			SetGuiColors ();
			SetLanguageToGui ();
			Initialize();

			if (!Constants.I.WINDOWS) {
				// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
				// Otherwise ShadowType.In looks terrible at Win10.
				frameCursorPos.ShadowType = ShadowType.In;
				frameComboboxes.ShadowType = ShadowType.In;
				frame_combobox1.ShadowType = ShadowType.In;
				frame_combobox2.ShadowType = ShadowType.In;
				frame_combobox3.ShadowType = ShadowType.In;
				frameHScales.ShadowType = ShadowType.In;
				frame_hscale1.ShadowType = ShadowType.In;
				frame_hscale2.ShadowType = ShadowType.In;
				frame_hscale3.ShadowType = ShadowType.In;
				frame_hscale4.ShadowType = ShadowType.In;
				frame_hscale5.ShadowType = ShadowType.In;
			}

			timeoutHandler = () => {
				filterProperties[0] = (double)combobox1.Active;
				filterProperties[1] = (double)combobox2.Active;
				filterProperties[2] = (double)combobox3.Active;

				filterProperties[3] = hscale1.Value;
				filterProperties[4] = hscale2.Value;
				filterProperties[5] = hscale3.Value;
				filterProperties[6] = hscale4.Value;
				filterProperties[7] = hscale5.Value;
				ProcessPreview ();

				repeatTimeout = false;
				return false;
			};
			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;
		}

		public FilterWidget (string pFilename, InvertFilter invert) : this (pFilename)
		{
			abstractFilter = invert;
			Title = Language.I.L [90];
			ProcessPreview ();
		}

		public FilterWidget (string pFilename, GrayscaleFilter grayscale) : this (pFilename)
		{
			abstractFilter = grayscale;

			Title = Language.I.L [91];
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[85] + "</b>";
			combobox1.AppendText(Language.I.L[86]);
			combobox1.AppendText(Language.I.L[87]);
			combobox1.AppendText(Language.I.L[88]);
			combobox1.AppendText(Language.I.L[89]);
			combobox1.Active = (int)grayscale.Algorithm;
		}

		public FilterWidget (string pFilename, ExtractOrRotateChannelsFilter extractOrRotateChannels) : this (pFilename)
		{
			abstractFilter = extractOrRotateChannels;

			Title = Language.I.L [92];
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[93] + "</b>";
			combobox1.AppendText(Language.I.L[94]);
			combobox1.AppendText(Language.I.L[95]);
			combobox1.AppendText(Language.I.L[96]);
			combobox1.AppendText(Language.I.L[97]);
			combobox1.AppendText(Language.I.L[98]);
			combobox1.AppendText(Language.I.L[99]);
			combobox1.AppendText(Language.I.L[100]);
			combobox1.AppendText(Language.I.L[101]);
			combobox1.AppendText(Language.I.L[102]);
			combobox1.AppendText(Language.I.L[103]);
			combobox1.Active = (int)extractOrRotateChannels.Order;
		}

		public FilterWidget (string pFilename, GaussianBlurFilter gaussianBlur, bool unsharpMasking) : this (pFilename)
		{			
			abstractFilter = gaussianBlur;
			Title = Language.I.L [104];
			useSameImageSizeLikePreview = true;

			SetGaussianBlurProperties (gaussianBlur.Sigma, gaussianBlur.Size);

			if (unsharpMasking) {
				gaussianBlur.UnsharpMasking = unsharpMasking;
				Title = Language.I.L [275];

				// Intensity of sharpness, [0.2, 4.0] (means 20% - 400%).
				frame_hscale3.Visible = true;
				lbFrame_hscale3.LabelProp = "<b>" + Language.I.L[276] + "</b>";
				hscale3.Value = gaussianBlur.Weight;
				hscale3.Adjustment.Lower = 0.2;
				hscale3.Adjustment.Upper = 4.0;
				hscale3.Adjustment.StepIncrement = 0.01;
				hscale3.Adjustment.PageIncrement = 0.2;
				hscale3.Digits = 2;
			}
		}			

		private void SetGaussianBlurProperties(double sigma, int size)
		{
			frameHScales.Visible = true;
			// Gaussian sigma value, [0.1, 7.0]. Default: 1.4
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[105] + "</b>";
			hscale1.Value = sigma;
			hscale1.Adjustment.Lower = 0.11;
			hscale1.Adjustment.Upper = 7.0;
			hscale1.Adjustment.StepIncrement = 0.01;
			hscale1.Adjustment.PageIncrement = 0.3;
			hscale1.Digits = 2;

			// Kernel size, [3, 11]. Default: 5
			frame_hscale2.Visible = true;
			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[106] + "</b>";
			hscale2.Value = size;
			hscale2.Adjustment.Lower = 3;
			hscale2.Adjustment.Upper = 11;
			hscale2.Adjustment.StepIncrement = 1;
			hscale2.Digits = 0;
		}

		public FilterWidget (string pFilename, CannyEdgeDetectorFilter cannyEdgeDetector) : this (pFilename)
		{
			abstractFilter = cannyEdgeDetector;
			Title = Language.I.L [108];
			useSameImageSizeLikePreview = true;

			SetGaussianBlurProperties (cannyEdgeDetector.Sigma, cannyEdgeDetector.Size);

			// LowThreshold. Default: 20
			frame_hscale3.Visible = true;
			lbFrame_hscale3.LabelProp = "<b>" + Language.I.L[113] + "</b>";
			hscale3.Value = cannyEdgeDetector.LowThreshold;
			hscale3.Adjustment.Lower = 1;
			hscale3.Adjustment.Upper = 100;
			hscale3.Adjustment.StepIncrement = 1;
			hscale3.Digits = 0;

			// HighThreshold. Default: 40
			frame_hscale4.Visible = true;
			lbFrame_hscale4.LabelProp = "<b>" + Language.I.L[114] + "</b>";
			hscale4.Value = cannyEdgeDetector.HighThreshold;
			hscale4.Adjustment.Lower = 1;
			hscale4.Adjustment.Upper = 100;
			hscale4.Adjustment.StepIncrement = 1;
			hscale4.Digits = 0;

			// OrientationColored. Default: false
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[115] + "</b>";
			combobox1.AppendText(Language.I.L[116]);
			combobox1.AppendText(Language.I.L[117]);
			combobox1.Active = cannyEdgeDetector.OrientationColored ? 0 : 1;
		}

		public FilterWidget (string pFilename, SepiaFilter sepia) : this (pFilename)
		{
			abstractFilter = sepia;
			Title = Language.I.L [120];
			frameHScales.Visible = true;

			// Q coefficient of YIQ color space [0.0, 10.0]. Default: 0.0
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[121] + "</b>";
			hscale1.Value = sepia.Q; // 0.0;
			hscale1.Adjustment.Lower = 0.0;
			hscale1.Adjustment.Upper = 10.0;
			hscale1.Adjustment.StepIncrement = 0.1;
			hscale1.Adjustment.PageIncrement = 1.0;
			hscale1.Digits = 1;

			// I coefficient of YIQ color space [1, 255]. Default: 51.
			frame_hscale2.Visible = true;
			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[122] + "</b>";
			hscale2.Value = sepia.I; // 51;
			hscale2.Adjustment.Lower = 1;
			hscale2.Adjustment.Upper = 255;
			hscale2.Adjustment.StepIncrement = 1;
			hscale2.Digits = 0;
		}

		public FilterWidget (string pFilename, OilPaintingFilter oilPainting) : this (pFilename)
		{
			abstractFilter = oilPainting;
			Title = Language.I.L [123];
			useSameImageSizeLikePreview = true;
			frameHScales.Visible = true;

			// Brush size to search for most frequent pixels' intensity. [1, 10]. Default: 5
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[124] + "</b>";
			hscale1.Value = oilPainting.BrushSize;
			hscale1.Adjustment.Lower = 1;
			hscale1.Adjustment.Upper = 25;
			hscale1.Adjustment.StepIncrement = 2;
			hscale1.Adjustment.PageIncrement = 4;
			hscale1.Digits = 0;
		}

		public FilterWidget (string pFilename, DifferenceFilter diff) : this (pFilename)
		{
			abstractFilter = diff;
			InitMultiImages ();
			Title = Language.I.L [150];
			frameHScales.Visible = true;

			// ThickPixel usage. Default: false
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[159] + "</b>";
			combobox1.AppendText(Language.I.L[117]);
			combobox1.AppendText(Language.I.L[116]);
			combobox1.Active = diff.DrawThick3x3Pixels ? 1 : 0;

			// 	Smallest allowed value in the resulting range [0, 255]. Default: 0. 
			// When <see cref="Highest"/> is also default value (255), no mapping is done.
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[157] + "</b>";
			hscale1.Adjustment.Lower = 0;
			hscale1.Adjustment.Upper = 255;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 5;
			hscale1.Value = diff.Smallest;

			// 	Highest allowed value in the resulting range [1, 255]. Default: 255. 
			// When <see cref="Smallest"/> is also default value (0), no mapping is done.
			frame_hscale2.Visible = true;
			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[158] + "</b>";
			hscale2.Adjustment.Lower = 1;
			hscale2.Adjustment.Upper = 255;
			hscale2.Adjustment.StepIncrement = 1;
			hscale2.Adjustment.PageIncrement = 5;
			hscale2.Value = diff.Highest;
		}

		public FilterWidget (string pFilename, MosaicFilter mosaic) : this (pFilename)
		{
			abstractFilter = mosaic;
			InitMultiImages ();
			Title = Language.I.L [307];

			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[322] + "</b>";
			combobox1.AppendText(Language.I.L[117]);
			combobox1.AppendText(Language.I.L[116]);
			combobox1.Active = mosaic.Inverted; // ? 1 : 0;

			frameHScales.Visible = true;
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L [314] + "</b>";
			hscale1.Adjustment.Lower = 2.0;
			hscale1.Adjustment.Upper = image.Width / 2;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 4;
//				hscale1.Digits = 2;
			hscale1.Value = mosaic.Number;
			
		}

		public FilterWidget (string pFilename, BlendFilter blend) : this (pFilename)
		{
			abstractFilter = blend;
			InitMultiImages ();
			Title = Language.I.L [305];

			if (blend.ImagesPaths.Length == 2) {
				frameHScales.Visible = true;

				frame_hscale1.Visible = true;
				lbFrame_hscale1.LabelProp = "<b>" + Language.I.L [323] + "</b>";
				hscale1.Adjustment.Lower = 0.0;
				hscale1.Adjustment.Upper = 100.0;
				hscale1.Adjustment.StepIncrement = 1;
				hscale1.Adjustment.PageIncrement = 10;
//				hscale1.Digits = 2;
				hscale1.Value = blend.MixPercent * 100.0;
			} else {
				ProcessPreview ();
			}
		}

		public FilterWidget (string pFilename, PosterizationFilter posterization) : this (pFilename)
		{
			abstractFilter = posterization;
			Title = Language.I.L [168];
			frameHScales.Visible = true;

			// Brush size to search for most frequent pixels' intensity. [1, 10]. Default: 5
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[169] + "</b>";
			hscale1.Value = posterization.Divisor;
			hscale1.Adjustment.Lower = 1;
			hscale1.Adjustment.Upper = 255;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 5;
			hscale1.Digits = 0;
		}

		public FilterWidget (string pFilename, CartoonFilter cartoon) : this (pFilename)
		{
			abstractFilter = cartoon;
			Title = Language.I.L [268];
			useSameImageSizeLikePreview = true;
			ProcessPreview ();

//			frameHScales.Visible = true;
////
//			// hue range
//			frame_hscale1.Visible = true;
//			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[269] + "</b>";
//			hscale1.Value = cartoon.HueRange;
//			hscale1.Adjustment.Lower = 1;
//			hscale1.Adjustment.Upper = 360;
//			hscale1.Adjustment.StepIncrement = 1;
//			hscale1.Adjustment.PageIncrement = 5;
//			hscale1.Digits = 0;
//
//			// saturation range
//			frame_hscale2.Visible = true;
//			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[270] + "</b>";
//			hscale2.Value = cartoon.SaturationRange;
//			hscale2.Adjustment.Lower = 1;
//			hscale2.Adjustment.Upper = 100;
//			hscale2.Adjustment.StepIncrement = 1;
//			hscale2.Adjustment.PageIncrement = 5;
//			hscale2.Digits = 0;
//
//			// lightness range
//			frame_hscale3.Visible = true;
//			lbFrame_hscale3.LabelProp = "<b>" + Language.I.L[271] + "</b>";
//			hscale3.Value = cartoon.LightnessRange;
//			hscale3.Adjustment.Lower = 1;
//			hscale3.Adjustment.Upper = 100;
//			hscale3.Adjustment.StepIncrement = 1;
//			hscale3.Adjustment.PageIncrement = 5;
//			hscale3.Digits = 0;
		}

		public FilterWidget (string pFilename, SobelEdgeDetectorFilter sobel) : this (pFilename)
		{
			abstractFilter = sobel;
			Title = Language.I.L [272];
			useSameImageSizeLikePreview = true;
			frameHScales.Visible = true;

			// Draw black white, not grayscale. Default: false
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[273] + "</b>";
			combobox1.AppendText(Language.I.L[117]);
			combobox1.AppendText(Language.I.L[116]);
			combobox1.Active = sobel.BlackWhite ? 1 : 0;

			// Sobel threshold
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[274] + "</b>";
			hscale1.Value = sobel.Threshold;
			hscale1.Adjustment.Lower = 0;
			hscale1.Adjustment.Upper = 255;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 5;
			hscale1.Digits = 0;
		}

		public FilterWidget (string pFilename, SobelEdgeMarkerFilter sobel) : this (pFilename)
		{
			abstractFilter = sobel;
			Title = Language.I.L [277];
			useSameImageSizeLikePreview = true;
			frameHScales.Visible = true;

			// Sktech edges in white instead black color. Default: false
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[278] + "</b>";
			combobox1.AppendText(Language.I.L[279]);
			combobox1.AppendText(Language.I.L[280]);
			combobox1.Active = sobel.UseWhiteEdgeColor ? 1 : 0;

			// Sobel threshold
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[274] + "</b>";
			hscale1.Adjustment.Lower = 0;
			hscale1.Adjustment.Upper = 255;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 5;
			hscale1.Digits = 0;
			hscale1.Value = sobel.Threshold;
		}

		public FilterWidget (string pFilename, BinarizationFilter filter) : this (pFilename)
		{
			abstractFilter = filter;
			Title = Language.I.L [285];
			frameHScales.Visible = true;
//			useSameImageSizeLikePreview = true;

			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[286] + "</b>";
			combobox1.AppendText(Language.I.L[287]);
			combobox1.AppendText(Language.I.L[288]);
			combobox1.Active = filter.ColorBinarization ? 1 : 0;

			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[289] + "</b>";
			hscale1.Adjustment.Lower = 0;
			hscale1.Adjustment.Upper = 255;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 5;
			hscale1.Digits = 0;
			hscale1.Value = filter.Threshold;
		}

		public FilterWidget (string pFilename, MeanshiftFilter filter) : this (pFilename)
		{
			abstractFilter = filter;
			Title = Language.I.L [290];
//			frameHScales.Visible = true;
			useSameImageSizeLikePreview = true;
			ProcessPreview ();
		}

		public FilterWidget (string pFilename, DilatationFilter filter) : this (pFilename)
		{
			abstractFilter = filter;
			Title = Language.I.L [292];
			ProcessPreview ();
		}

		public FilterWidget (string pFilename, ExponentiateChannelsFilter filter) : this (pFilename)
		{
			abstractFilter = filter;
			Title = Language.I.L [293];
			//			useSameImageSizeLikePreview = true;

			frameHScales.Visible = true;
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[293] + "</b>";
			hscale1.Adjustment.Lower = 0.1;
			hscale1.Adjustment.Upper = 4.0;
			hscale1.Adjustment.StepIncrement = 0.01;
			hscale1.Adjustment.PageIncrement = 0.1;
			hscale1.Digits = 2;
			hscale1.Value = filter.Exponent;

			frame_hscale2.Visible = true;
			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[289] + "</b>";
			hscale2.Adjustment.Lower = 0;
			hscale2.Adjustment.Upper = 255;
			hscale2.Adjustment.StepIncrement = 1;
			hscale2.Adjustment.PageIncrement = 5;
			hscale2.Digits = 0;
			hscale2.Value = filter.Threshold;
		}

		public FilterWidget (string pFilename, Convolution5X5Filter filter) : this (pFilename)
		{
			abstractFilter = filter;
			Title = Language.I.L [295];
			useSameImageSizeLikePreview = true;

			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[296] + "</b>";
			combobox1.AppendText(Language.I.L[297]);
			combobox1.AppendText(Language.I.L[298]);
			combobox1.AppendText(Language.I.L[299]);
			combobox1.AppendText(Language.I.L[300]);
			combobox1.Active = (int)filter.Mask;
		}

		public FilterWidget (string pFilename, MirrorFilter mirror) : this (pFilename)
		{
			abstractFilter = mirror;

			Title = Language.I.L [308];
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[312] + "</b>";
			combobox1.AppendText(Language.I.L[309]);
			combobox1.AppendText(Language.I.L[310]);
			combobox1.AppendText(Language.I.L[311]);
			combobox1.Active = (int)mirror.Axis;
		}

		public FilterWidget (string pFilename, ChessboardFilter chess) : this (pFilename)
		{
			abstractFilter = chess;

			Title = Language.I.L [313];

			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[315] + "</b>";
			combobox1.AppendText(Language.I.L[316]);
			combobox1.AppendText(Language.I.L[317]);
			combobox1.AppendText(Language.I.L[318]);
			combobox1.AppendText(Language.I.L[319]);
			combobox1.AppendText(Language.I.L[320]);
			combobox1.AppendText(Language.I.L[321]);
			combobox1.Active = (int)chess.Variants;

			frameHScales.Visible = true;
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[314] + "</b>";
			hscale1.Adjustment.Lower = 2.0;
			hscale1.Adjustment.Upper = 40.0;
			hscale1.Adjustment.StepIncrement = 1;
			hscale1.Adjustment.PageIncrement = 4;
//			hscale1.Digits = 2;
			hscale1.Value = chess.Number;

		}

		#endregion Constructors

		public override void Destroy ()
		{
			if (image != null) {
				image.Dispose ();
			}

			if (!keepFilterImageInRam && filterImage != null) {
				filterImage.Dispose ();
			}

			base.Destroy ();
		}

		private void ProcessPreview()
		{
			Bitmap b;
			try {
				filterImage = abstractFilter.Apply (image, filterProperties);

				ImageConverter.ScaleAndCut (
					filterImage, 
					out b, 
					0,
					0,
					simpleimagepanel1.WidthRequest,
					simpleimagepanel1.HeightRequest,
					ConvertMode.StretchForge,
					false);
			}
			catch(ArgumentException) {
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [125];
				pseudo.Label1 = string.Empty; // Language.I.L [125];
				pseudo.Label2 = Language.I.L [126];
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.SetPosition (WindowPosition.CenterAlways);
//				pseudo.Show ();
				this.DestroyAll ();
				return;
			}

			b.Save(simpleimagepanel1.MemoryStream, ImageFormat.Png);
			b.Dispose ();
			simpleimagepanel1.Initialize();
		}

		private void ProcessFilter()
		{
			Bitmap b;
			if (useSameImageSizeLikePreview) {
				//				b = abstractFilter.Apply (image, filterProperties);
				PixelFormat pf = filterImage.PixelFormat;

				if (filterImage.Width != imageW || filterImage.Height != imageH) {

					ImageConverter.ScaleAndCut (
						filterImage, 
						out b, 
						0,
						0,
						imageW,
						imageH,
						ConvertMode.StretchForge,
						true);

					if (pf == PixelFormat.Format8bppIndexed) {
						b = ImageConverter.To8Bpp (b);
					}
				} else {
					b = filterImage;
					keepFilterImageInRam = true;
				}
			} else {
				image = TroonieBitmap.FromFile (FileName);

				if (abstractFilter is IMultiImagesFilter) {
					IMultiImagesFilter imf = abstractFilter as IMultiImagesFilter;
					imf.DisposeImages ();
					InitMultiImages ();
				}
				b = abstractFilter.Apply (image, filterProperties);
			}
			FireFilterEvent (b);
			this.DestroyAll ();
		}

		private void Initialize()
		{
			Title = FileName;
			Bitmap b = TroonieBitmap.FromFile (FileName);
			imageW = b.Width;
			imageH = b.Height;
			int w, h;
			// get full size, also all gui elements are not visible
			this.vboxA.GdkWindow.GetSize(out w, out h);
			GuiHelper.I.SetPanelSize(this, simpleimagepanel1, hbox1, 400, 300, imageW, imageH, w, h);	

			ImageConverter.ScaleAndCut (
				b, 
				out image, 
				0,
				0,
				simpleimagepanel1.WidthRequest,
				simpleimagepanel1.HeightRequest,
				ConvertMode.StretchForge,
				false);

			image.Save(simpleimagepanel1.MemoryStream, ImageFormat.Png);
			simpleimagepanel1.Initialize();

			// prepare for ProcessPreview
			if (imageW > Constants.I.CONFIG.MaxImageLengthForFiltering || 
				imageH > Constants.I.CONFIG.MaxImageLengthForFiltering) {
				float div = Math.Max (imageW, imageH) / (float)Constants.I.CONFIG.MaxImageLengthForFiltering;
				int sw = (int)Math.Round (imageW / div);
				int sh = (int)Math.Round (imageH / div);

				ImageConverter.ScaleAndCut (
					b, 
					out image, 
					0,
					0,
					sw,
					sh,
					ConvertMode.StretchForge,
					true);

				if (b.PixelFormat == PixelFormat.Format8bppIndexed) {
					image = ImageConverter.To8Bpp (image);
				}
			} else {
				image = TroonieBitmap.FromFile (FileName);
			}

			b.Dispose ();

			// do not use, otherwise all invisible widgets becomes visible
//			ShowAll();
		}		

		private void InitMultiImages()
		{
			// prepare for ProcessPreview, if filter is IMultiImagesFilter
			if (abstractFilter is IMultiImagesFilter) {
				IMultiImagesFilter imf = abstractFilter as IMultiImagesFilter;
				imf.Images = new Bitmap[imf.ImagesPaths.Length];
				imf.Images [0] = image;

				for (int i = 1; i < imf.ImagesPaths.Length; i++) {
					Bitmap b = TroonieBitmap.FromFile (imf.ImagesPaths[i]); 
					if (image.Width != b.Width || image.Height != b.Height) {
						ImageConverter.ScaleAndCut (
							b, 
							out imf.Images[i], 
							0,
							0,
							image.Width,
							image.Height,
							ConvertMode.StretchForge,
							true);
					}
					else { // same image size 
						imf.Images[i] = b;
					}

					if (image.PixelFormat == PixelFormat.Format8bppIndexed) {
						imf.Images[i] = ImageConverter.To8Bpp (imf.Images[i]);
					}
				} // end for loop
			}
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);

			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);

//			lbFrameHScales.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_combobox1.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_combobox2.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_combobox3.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale1.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale2.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale3.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale4.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale5.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

//			lbFrameHScales.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[78] + "</b>";

			checkBtnUse255ForAlpha.Label = Language.I.L[107] ;
		}

		private void FireFilterEvent(Bitmap filterBitmap)
		{
			//fire the event now
			if (FilterEvent != null) //is there a EventHandler?
			{
				FilterEvent.Invoke(filterBitmap); //calls its EventHandler                
			}
			else { } //if not, ignore
		}

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	x.ToString() + " x " +	y.ToString();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();
//			File.Delete (tempFilterImageFileName);
		}

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
//			const int maxRes = 1100 * 1100;
//
//			if (filterImage.Width * filterImage.Height > maxRes && 
//			    (abstractFilter is OilPaintingFilter ||
//			 	 abstractFilter is CannyEdgeDetectorFilter ||
//			     abstractFilter is GaussianBlurFilter) ) {
//				OkCancelDialog warn = new OkCancelDialog (false);
//				warn.Title = Language.I.L [29];
//				warn.Label1 = Language.I.L [31];
//				warn.Label2 = Language.I.L [33];
//				warn.OkButtontext = Language.I.L [16];
//				warn.CancelButtontext = Language.I.L [17];	
//				warn.Show ();
//
//				warn.OnReleasedOkButton += ProcessFilter;
//			} else {
				ProcessFilter ();
//			}
		}

		protected void OnBtnCancelButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			this.DestroyAll ();
		}

		protected void OnCheckBtnUse255ForAlphaToggled (object sender, EventArgs e)
		{
			abstractFilter.Use255ForAlpha = checkBtnUse255ForAlpha.Active;
			ProcessPreview ();
		}

		protected void OnHscaleValueChanged (object sender, EventArgs e)
		{
			if (repeatTimeout)
				return;

			repeatTimeout = true;
			GLib.Timeout.Add(Constants.TIMEOUT_FILTER_PROCESS_PREVIEW, timeoutHandler);
		}

		protected void OnComboboxChanged (object sender, EventArgs args)
		{
//			timeoutHandler.Invoke ();

			if (repeatTimeout)
				return;

			repeatTimeout = true;
			GLib.Timeout.Add(0, timeoutHandler);
		}			
	}
}

