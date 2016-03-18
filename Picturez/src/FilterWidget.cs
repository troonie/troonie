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
		private const uint timeoutInterval = 500; // in ms

		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
//		private Constants constants = Constants.I;
		private bool repeatTimeout;
		private int imageW; 
		private int imageH;
		private string tempFilterImageFileName;
		private Bitmap filterImage, workingImage;

		#region filter
		private AbstractFilter abstractFilter;
		private GrayscaleFilter grayscale;
//		private InvertFilter invert;
		private ExtractOrRotateChannelsFilter extractOrRotateChannels;
		private GaussianBlurFilter gaussianBlur;
		CannyEdgeDetectorFilter cannyEdgeDetector;
		#endregion filter

		public FilterEventhandler FilterEvent;
		public string FileName { get; set; }

		#region Constructors

		protected FilterWidget (string pFilename) : base (Gtk.WindowType.Toplevel)
		{
			FileName = pFilename;
			KeepAbove = true;
			Build ();

			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

			SetGuiColors ();
			SetLanguageToGui ();
			Initialize();

			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;
		}

		public FilterWidget (string pFilename, InvertFilter invert) : this (pFilename)
		{
//			this.invert = invert;
			abstractFilter = invert;

			Title = Language.I.L [90];
			ProcessPreview ();
		}

		public FilterWidget (string pFilename, GrayscaleFilter grayscale) : this (pFilename)
		{
			this.grayscale = grayscale;
			abstractFilter = grayscale;

			Title = Language.I.L [91];
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[85] + "</b>";
			combobox1.AppendText(Language.I.L[86]);
			combobox1.AppendText(Language.I.L[87]);
			combobox1.AppendText(Language.I.L[88]);
			combobox1.AppendText(Language.I.L[89]);
			combobox1.Active = 0;
			combobox1.Changed += new EventHandler (Grayscale_Combobox1Changed);

			ProcessPreview ();
		}

		public FilterWidget (string pFilename, ExtractOrRotateChannelsFilter extractOrRotateChannels) : this (pFilename)
		{
			this.extractOrRotateChannels = extractOrRotateChannels;
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

			combobox1.Active = 8; // GBR
			combobox1.Changed += new EventHandler (ExtractOrRotateChannels_Combobox1Changed);

			ProcessPreview ();
		}

		public FilterWidget (string pFilename, GaussianBlurFilter gaussianBlur) : this (pFilename)
		{
			this.gaussianBlur = gaussianBlur;
			abstractFilter = gaussianBlur;

			Title = Language.I.L [104];
			SetGaussianBlurProperties (GaussianBlur_Hscale1ChangeValue, GaussianBlur_Hscale2ChangeValue);
			ProcessPreview ();
		}

		private void SetGaussianBlurProperties(ChangeValueHandler changeValue1, ChangeValueHandler changeValue2)
		{
			frameHScales.Visible = true;
			// Gaussian sigma value, [0.1, 7.0]. Default: 1.4
			frame_hscale1.Visible = true;
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[105] + "</b>";
			hscale1.Value = 1.40;
			hscale1.Adjustment.Lower = 0.11;
			hscale1.Adjustment.Upper = 7.0;
			hscale1.Adjustment.StepIncrement = 0.01;
			hscale1.Digits = 2;
			hscale1.ChangeValue += changeValue1;
			// Kernel size, [3, 11]. Default: 5
			frame_hscale2.Visible = true;
			lbFrame_hscale2.LabelProp = "<b>" + Language.I.L[106] + "</b>";
			hscale2.Value = 5;
			hscale2.Adjustment.Lower = 3;
			hscale2.Adjustment.Upper = 11;
			hscale2.Adjustment.StepIncrement = 1;
			hscale2.Digits = 0;
			hscale2.ChangeValue += changeValue2;
		}

		public FilterWidget (string pFilename, CannyEdgeDetectorFilter cannyEdgeDetector) : this (pFilename)
		{
			this.cannyEdgeDetector = cannyEdgeDetector;
			abstractFilter = cannyEdgeDetector;

			Title = Language.I.L [108];
			SetGaussianBlurProperties (CannyEdgeDetector_Hscale1ChangeValue, CannyEdgeDetector_Hscale2ChangeValue);

			// OrientationColored. Default: false
			frameComboboxes.Visible = true;
			frame_combobox1.Visible = true;
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[115] + "</b>";
			combobox1.AppendText(Language.I.L[116]);
			combobox1.AppendText(Language.I.L[117]);
			combobox1.Active = 1;
			combobox1.Changed += new EventHandler (CannyEdgeDetector_Combobox1Changed);

			// LowThreshold. Default: 20
			frame_hscale3.Visible = true;
			lbFrame_hscale3.LabelProp = "<b>" + Language.I.L[113] + "</b>";
			hscale3.Value = 20;
			hscale3.Adjustment.Lower = 1;
			hscale3.Adjustment.Upper = 100;
			hscale3.Adjustment.StepIncrement = 1;
			hscale3.Digits = 0;
			hscale3.ChangeValue += CannyEdgeDetector_Hscale3ChangeValue;

			// HighThreshold. Default: 40
			frame_hscale4.Visible = true;
			lbFrame_hscale4.LabelProp = "<b>" + Language.I.L[114] + "</b>";
			hscale4.Value = 40;
			hscale4.Adjustment.Lower = 1;
			hscale4.Adjustment.Upper = 100;
			hscale4.Adjustment.StepIncrement = 1;
			hscale4.Digits = 0;
			hscale4.ChangeValue += CannyEdgeDetector_Hscale4ChangeValue;

			ProcessPreview ();
		}

		#endregion Constructors

		private void ProcessPreview()
		{
			Bitmap tempImage = abstractFilter.Apply (workingImage);
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
			filterImage = abstractFilter.Apply (filterImage);
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
	}
}

