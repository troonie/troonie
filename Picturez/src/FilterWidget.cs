using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Gtk;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using IOPath = System.IO.Path;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using ImageConverter = Picturez_Lib.ImageConverter;
using Picturez;
using Picturez_Lib;
using System.Diagnostics;

namespace Picturez
{
	public delegate void FilterEventhandler(Bitmap filterBitmap);

	public partial class FilterWidget : Gtk.Window
	{
		private const uint timeoutInterval = 300; // in ms

		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
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

		#endregion Constructors

		private void ProcessPreview()
		{
			Bitmap tempImage = abstractFilter.Apply (workingImage, filterProperties);
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

			SetPanelSize();	

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

		private void SetPanelSize()
		{		
			const int optionsWidth = 390;
			// general taskbar size in win_8.1
			const int taskbarHeight = 90;
			const int paddingOffset = 44;
			// necessary to correct to small height 
			const float multiplicatorHeight = 1.2f;

			Gdk.Screen screen = this.Screen;
			int monitor = screen.GetMonitorAtWindow (this.GdkWindow); 
			Gdk.Rectangle bounds = screen.GetMonitorGeometry (monitor);
			int winW = bounds.Width;
			// DIFFERENCE 1 to EditWidget
			//			int winH = bounds.Height - taskbarHeight - 300;
			//			int winW = 700;
			int winH = 600;

			// DIFFERENCE 2 to EditWidget
			//			int panelW = winW - optionsWidth - paddingOffset;
			//			int panelH = winH - (int)(paddingOffset * multiplicatorHeight);
			int panelW = 300;
			int panelH = 200;

			// setting padding for left and right side
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.simpleimagepanel1]));
			w4.Padding = ((uint)(paddingOffset / 4.0f + 0.5f));

			if (panelW < imageW || panelH < imageH)
			{
				bool wLonger = (imageW / (float)imageH) > (panelW / (float)panelH);
				if (wLonger)
				{
					panelH = (int)(imageH * panelW / (float)imageW  + 0.5f);
					winH = panelH + (int)(paddingOffset * multiplicatorHeight);
				}
				else
				{
					panelW = (int)(imageW * panelH / (float)imageH  + 0.5f);
					winW = panelW + optionsWidth + paddingOffset;
				}
			}
			else
			{
				panelW = imageW;
				panelH = imageH;
				winW = panelW + optionsWidth + paddingOffset;
				winH = panelH + (int)(paddingOffset * multiplicatorHeight);
			}						

			simpleimagepanel1.WidthRequest = panelW;
			simpleimagepanel1.HeightRequest = panelH;

			simpleimagepanel1.ScaleCursorX = imageW / (float)panelW;
			simpleimagepanel1.ScaleCursorY = imageH / (float)panelH;

			this.Resize (winW, winH);
			this.Move (0, 0);
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
			workingImage.Dispose ();
			// Do not dispose it here!
//			filterImage.Dispose ();
			simpleimagepanel1.Dispose ();
			this.Destroy ();
//			this.Dispose ();
			File.Delete (tempFilterImageFileName);
		}

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			filterImage = abstractFilter.Apply (filterImage, filterProperties);
			FireFilterEvent (filterImage);
			OnDeleteEvent (o, null);
		}

		protected void OnBtnCancelButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			OnDeleteEvent (o, null);
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
			GLib.Timeout.Add(timeoutInterval, timeoutHandler);
		}

		protected void OnComboboxChanged (object sender, EventArgs args)
		{
			timeoutHandler.Invoke ();
		}
	}
}

