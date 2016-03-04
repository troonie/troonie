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
	public partial class FilterWidget : Gtk.Window
	{
		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
//		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempFilterImageFileName;

		#region filter
		private GrayscaleFilter grayscale;
		private InvertFilter invert;
		#endregion filter

		public string FileName { get; set; }


		public FilterWidget (string pFilename, InvertFilter invert) : this (pFilename)
		{
			this.invert = invert;
		}

		public FilterWidget (string pFilename, GrayscaleFilter grayscale) : this (pFilename)
		{
			this.grayscale = grayscale;
		}

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

		private void Initialize()
		{
			Title = FileName;
			Bitmap pic = new Bitmap(FileName); 
			imageW = pic.Width;
			imageH = pic.Height;

			SetPanelSize();	

			tempFilterImageFileName = Constants.I.EXEPATH + "tempFilterImageFileName.png";

			simpleimagepanel1.SurfaceFileName = tempFilterImageFileName;
						                            
			Bitmap croppedPic;

			ImageConverter.ScaleAndCut (
				pic, 
				out croppedPic, 
				0,
				0,
				simpleimagepanel1.WidthRequest,
				simpleimagepanel1.HeightRequest,
				ConvertMode.StretchForge,
				false);

			pic.Dispose ();
			croppedPic.Save(tempFilterImageFileName, ImageFormat.Png);
			croppedPic.Dispose();

			simpleimagepanel1.Initialize();

			ShowAll();
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

			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_combobox1.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrame_hscale1.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			lbFrameSteganography.LabelProp = "<b>" + Language.I.L[73] + "</b>";
			lbFrameModus.LabelProp = "<b>" + Language.I.L[74] + "</b>";
			rdBtnRead.Label = Language.I.L[75];
			rdBtnWrite.Label = Language.I.L[76];
			lbFrame_combobox1.LabelProp = "<b>" + Language.I.L[77] + "</b>";
			lbFrame_hscale1.LabelProp = "<b>" + Language.I.L[78] + "</b>";
		}

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	x.ToString() + " x " +	y.ToString();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			simpleimagepanel1.Dispose ();
			this.Dispose ();
			File.Delete (tempFilterImageFileName);
		}

		protected void OnExit (object sender, EventArgs e)
		{	
			OnDeleteEvent (sender, e as DeleteEventArgs);
		}

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{

		}
	}
}

