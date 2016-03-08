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

namespace Picturez
{
	public delegate void FilterEventhandler(Bitmap filterBitmap);

	public partial class FilterWidget : Gtk.Window
	{
		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
//		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempFilterImageFileName;
		private Bitmap filterImage, workingImage;

		#region filter
		private AbstractFilter abstractFilter;
		private GrayscaleFilter grayscale;
		private InvertFilter invert;
		private ExtractOrRotateChannelsFilter extractOrRotateChannels;
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
			this.invert = invert;
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
			combobox1.Changed += new EventHandler (Grayscale_OnCombobox1Changed);

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
			combobox1.Changed += new EventHandler (ExtractOrRotateChannels_OnCombobox1Changed);

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
			lbFrame_hscale1.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

//			lbFrameHScales.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[78] + "</b>";
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

//		protected void OnExit (object sender, EventArgs e)
//		{	
//			OnDeleteEvent (sender, e as DeleteEventArgs);
//		}

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

		protected void Grayscale_OnCombobox1Changed (object sender, EventArgs e)
		{
			grayscale.Algorithm = (GrayscaleFilter.CommonAlgorithms) combobox1.Active;
			ProcessPreview ();
		}

		protected void ExtractOrRotateChannels_OnCombobox1Changed (object sender, EventArgs e)
		{
			//			Console.WriteLine ("Changed index: " + combobox1.Active);
			extractOrRotateChannels.Order = (ExtractOrRotateChannelsFilter.RGBOrder) combobox1.Active;
			ProcessPreview ();
		}
	}
}

