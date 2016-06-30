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
		private bool repeatTimeout;
		private int imageW; 
		private int imageH;
		private string tempFilterImageFileName;
		private Bitmap filterImage, workingImage;
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

		public FilterWidget (string pFilename, GaussianBlurFilter gaussianBlur) : this (pFilename)
		{
			abstractFilter = gaussianBlur;

			Title = Language.I.L [104];
			SetGaussianBlurProperties (gaussianBlur.Sigma, gaussianBlur.Size);
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

		#endregion Constructors

		public override void Destroy ()
		{
			// Do not dispose it here! It will be given to EditWidget by 
			// FilterEventhandler, see FireFilterEvent(..)-method
//			if (filterImage != null) {
//				filterImage.Dispose ();
//			}

			if (tempFilterImageFileName != null) {
				File.Delete (tempFilterImageFileName);
			}

			if (workingImage != null) {
				workingImage.Dispose ();
			}

			base.Destroy ();
		}

		private void ProcessPreview()
		{
			Bitmap tempImage;
			try {
				tempImage = abstractFilter.Apply (workingImage, filterProperties);
			}
			catch(ArgumentException) {
				PseudoTroonieContextMenu pseudo = new PseudoTroonieContextMenu (true);
				pseudo.Title = Language.I.L [125];
				pseudo.Label1 = string.Empty; // Language.I.L [125];
				pseudo.Label2 = Language.I.L [126];
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.SetPosition (WindowPosition.CenterAlways);
//				pseudo.Show ();
				this.DestroyAll ();
				return;
			}
			tempImage.Save(tempFilterImageFileName, ImageFormat.Png);
			tempImage.Dispose ();
			simpleimagepanel1.Initialize();
		}

		private void Initialize()
		{
			Title = FileName;
			filterImage = new Bitmap(FileName); 
			imageW = filterImage.Width;
			imageH = filterImage.Height;

			GuiHelper.I.SetPanelSize(this, simpleimagepanel1, hbox1, 400, 300, imageW, imageH);	

			tempFilterImageFileName = Constants.I.EXEPATH + "tempFilterImageFileName.png";

			ImageConverter.ScaleAndCut (
				filterImage, 
				out workingImage, 
				0,
				0,
				simpleimagepanel1.WidthRequest,
				simpleimagepanel1.HeightRequest,
				ConvertMode.StretchForge,
				false);

			workingImage.Save(tempFilterImageFileName, ImageFormat.Png);

			simpleimagepanel1.SurfaceFileName = tempFilterImageFileName;
			simpleimagepanel1.Initialize();

			// do not use, otherwise all invisible widgets becomes visible
//			ShowAll();
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
			const int maxRes = 1100 * 1100;

			if (filterImage.Width * filterImage.Height > maxRes && 
			    (abstractFilter is OilPaintingFilter ||
			 	 abstractFilter is CannyEdgeDetectorFilter ||
			     abstractFilter is GaussianBlurFilter) ) {
				PseudoTroonieContextMenu warn = new PseudoTroonieContextMenu (false);
				warn.Title = Language.I.L [29];
				warn.Label1 = Language.I.L [31];
				warn.Label2 = Language.I.L [33];
				warn.OkButtontext = Language.I.L [16];
				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				warn.OnReleasedOkButton += ProcessFilter;
			} else {
				ProcessFilter ();
			}
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
			timeoutHandler.Invoke ();
		}

		private void ProcessFilter()
		{
			filterImage = abstractFilter.Apply (filterImage, filterProperties);
			FireFilterEvent (filterImage);
			this.DestroyAll ();
		}
	}
}

