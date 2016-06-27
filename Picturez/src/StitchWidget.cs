using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Cairo;
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
	public partial class StitchWidget : Gtk.Window
	{ 
		private const string blackFileName = "black.png";
		private const int maxpadding = 100;

		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;
		private Constants constants = Constants.I;
		float imageScaleFactor;
		private int origImage01W, origImage01H, origImage02W, origImage02H;
		private string tempStitchImageFileName;
		private Bitmap workingImage;
		/// <summary>The scaled image 01. </summary>
		private Bitmap image01;
		/// <summary>The scaled image 02. </summary>
		private Bitmap image02;
		#region Stopwatch for picturez buttons
		private Stopwatch timeoutSw;
		private bool incrementValue;
		private bool repeatTimeoutStopwatch;
		#endregion Stopwatch for picturez buttons

		private Label pointerLabel;
		private StitchMIFilter st;
		#region Glib.Timout for proccessing preview
		private GLib.TimeoutHandler timeoutHandlerPreprocessing;
		private bool repeatTimeoutPreprocessing;
//		private bool preprocessingNecessary;
		#endregion Glib.Timout for proccessing preview

		public FilterEventhandler FilterEvent;
		public string FileName01 { get; set; }
		public string FileName02 { get; set; }

		public StitchWidget (string pFilename01, string pFilename02) : base (Gtk.WindowType.Toplevel)
		{
			FileName01 = pFilename01;
			FileName02 = pFilename02;
			tempStitchImageFileName = Constants.I.EXEPATH + "PreStitchImageFileName.png";
			KeepAbove = true;
			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);
			Title = FileName01 + " & " + FileName02;
			SetGuiColors ();
			SetLanguageToGui ();
			simpleimagepanel1.SurfaceFileName = tempStitchImageFileName;
			timeoutSw = new Stopwatch();

			CalcWorkingImages ();

			ProcessPreview();

		if (constants.WINDOWS) {
			Gtk.Drag.DestSet (this, 0, null, 0);
		} else {
			// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
			// Otherwise ShadowType.In looks terrible at Win10.
			frameCursorPos.ShadowType = ShadowType.In;
			frameImageResolution.ShadowType = ShadowType.In;
			frameStitch.ShadowType = ShadowType.In;
			frameModus.ShadowType = ShadowType.In;
			frameImagePositions.ShadowType = ShadowType.In;
			frameImagePositions2.ShadowType = ShadowType.In;
			Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
		}
			timeoutHandlerPreprocessing = ProcessPreview;
			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;
		}

		public override void Destroy ()
		{
			if (tempStitchImageFileName != null) {
				File.Delete (tempStitchImageFileName);
			}

			if (workingImage != null) {
				workingImage.Dispose ();
			}

			base.Destroy ();
		}
			
		private void CalcWorkingImages()
		{
			const int maxLength = 500;
			Bitmap b1, b2;
			// fast reading-out of image sizes
			using (FileStream fs = new FileStream (FileName01, FileMode.Open)) 
			{
				b1 = Image.FromStream (fs, true, false) as Bitmap;
				origImage01W = b1.Width;
				origImage01H = b1.Height;
			}
			using (FileStream fs = new FileStream (FileName02, FileMode.Open)) 
			{
				b2 = Image.FromStream (fs, true, false) as Bitmap;
				origImage02W = b2.Width;
				origImage02H = b2.Height;
			}

			int biggestLength = Math.Max(
				Math.Max (origImage01W, origImage01H), Math.Max (origImage02W, origImage02H));
			imageScaleFactor = biggestLength > maxLength ? biggestLength / maxLength : 1;

			ImageConverter.ScaleAndCut (
				b1, 
				out image01, 
				0,
				0,
				(int)(origImage01W / imageScaleFactor + 0.5f),
				(int)(origImage01H / imageScaleFactor + 0.5f),
				ConvertMode.StretchForge,
				false);

			ImageConverter.ScaleAndCut (
				b2, 
				out image02, 
				0,
				0,
				(int)(origImage02W / imageScaleFactor + 0.5f),
				(int)(origImage02H / imageScaleFactor + 0.5f),
				ConvertMode.StretchForge,
				false);

			b1.Dispose ();
			b2.Dispose ();
		}
			
		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbImageResolution.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);

			lbFrameStitch.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameImageResolution.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameImagePositions.ModifyFg (StateType.Normal, colorConverter.FONT);
			lbFrameImagePositions2.ModifyFg (StateType.Normal, colorConverter.FONT);

			lb01Bottom.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb01Left.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb01Right.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb01Top.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb02Bottom.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb02Left.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb02Right.ModifyFg (StateType.Normal, colorConverter.FONT);
			lb02Top.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			btnOk.Text = Language.I.L[16];
			btnOk.Redraw ();

			lbFrameStitch.LabelProp = "<b>" + Language.I.L[131] + "</b>";
			lbFrameImageResolution.LabelProp = "<b>" + Language.I.L[12] + "</b>";
			lbFrameModus.LabelProp = "<b>" + Language.I.L[74] + "</b>";
			rdBtnLandscape.Label = Language.I.L[129];
			rdBtnPortrait.Label = Language.I.L[130];
			lbFrameImagePositions.LabelProp = "<b>" + Language.I.L[132] + "</b>";
			lbFrameImagePositions2.LabelProp = "<b>" + Language.I.L[133] + "</b>";
		}

		private void SetImagePaddingAndLabel()
		{
			int v = int.Parse (pointerLabel.Text);
			if (incrementValue && v < maxpadding) {
				v++;
			}
			else if (!incrementValue && v > 0) {
				v--;
			}
			pointerLabel.Text = v.ToString ();
		}

		private bool SetImagePaddingAndLabelByTimeoutHandler()
		{
			if (timeoutSw.ElapsedMilliseconds < Constants.TIMEOUT_INTERVAL_FIRST) {
				return repeatTimeoutStopwatch;
			}

			SetImagePaddingAndLabel();
			return repeatTimeoutStopwatch;
		}
			
		private void FireFilterEvent(Bitmap resultBitmap)
		{
			//fire the event now
			if (FilterEvent != null) //is there a EventHandler?
			{
				FilterEvent.Invoke(resultBitmap); //calls its EventHandler                
			}
			else { } //if not, ignore
		}

		private bool ProcessPreview()
		{
			try {
				#region StitchMIFilter
				st = new StitchMIFilter(image01, image02);
				st.Landscape = rdBtnLandscape.Active;
				st.Left01 = (int)(int.Parse(lb01Left.Text) / imageScaleFactor + 0.5f);
				st.Left02 = (int)(int.Parse(lb02Left.Text) / imageScaleFactor + 0.5f);
				st.Right01 = (int)(int.Parse(lb01Right.Text) / imageScaleFactor + 0.5f);
				st.Right02 = (int)(int.Parse(lb02Right.Text) / imageScaleFactor + 0.5f);
				st.Top01 = (int)(int.Parse(lb01Top.Text) / imageScaleFactor + 0.5f);
				st.Top02 = (int)(int.Parse(lb02Top.Text) / imageScaleFactor + 0.5f);
				st.Bottom01 = (int)(int.Parse(lb01Bottom.Text) / imageScaleFactor + 0.5f);
				st.Bottom02 = (int)(int.Parse(lb02Bottom.Text) / imageScaleFactor + 0.5f);
	
				st.Process();
//				st.ResultBitmap.Save ("StitchMIFilter_pre.png", System.Drawing.Imaging.ImageFormat.Png);
				#endregion
			}
			catch(ArgumentException) {
				PseudoPicturezContextMenu pseudo = new PseudoPicturezContextMenu (true);
				pseudo.Title = Language.I.L [125];
				pseudo.Label1 = string.Empty; // Language.I.L [125];
				pseudo.Label2 = Language.I.L [126];
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.SetPosition (WindowPosition.CenterAlways);
				//				pseudo.Show ();
				this.DestroyAll ();
				repeatTimeoutPreprocessing = false;
				return false;
			}

			GuiHelper.I.SetPanelSize(this, simpleimagepanel1, hbox1, 400, 300, st.ResultBitmap.Width, st.ResultBitmap.Height, 600, 500);	
			ImageConverter.ScaleAndCut (
				st.ResultBitmap, 
				out workingImage, 
				0,
				0,
				simpleimagepanel1.WidthRequest,
				simpleimagepanel1.HeightRequest,
				ConvertMode.StretchForge,
				false);
			workingImage.Save (tempStitchImageFileName, System.Drawing.Imaging.ImageFormat.Png);

			simpleimagepanel1.Initialize();

			lbImageResolution.Text = ((int)(st.ResultBitmap.Width * imageScaleFactor + 0.5f)) + " x " +	
									 ((int)(st.ResultBitmap.Height * imageScaleFactor + 0.5f));

			repeatTimeoutPreprocessing = false;
			return false;
		}

		#region gui events

		private void OnCursorPosChanged(int x, int y)
		{
			lbCursorPos.Text = 	((int)(x * imageScaleFactor + 0.5f)).ToString() + " x " +	
				((int)(y * imageScaleFactor + 0.5f)).ToString();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();
		}			

		protected void OnRdBtnToggled (object sender, EventArgs e)
		{
			ProcessPreview();
		}

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			image01 = new Bitmap (FileName01);
			image02 = new Bitmap (FileName02);

			try {
				#region StitchMIFilter
				st = new StitchMIFilter(image01, image02);
				st.Landscape = rdBtnLandscape.Active;
				st.Left01 = int.Parse(lb01Left.Text);
				st.Left02 = int.Parse(lb02Left.Text);
				st.Right01 = int.Parse(lb01Right.Text);
				st.Right02 = int.Parse(lb02Right.Text);
				st.Top01 = int.Parse(lb01Top.Text);
				st.Top02 = int.Parse(lb02Top.Text);
				st.Bottom01 = int.Parse(lb01Bottom.Text);
				st.Bottom02 = int.Parse(lb02Bottom.Text);

				st.Process();
//				st.ResultBitmap.Save ("StitchMIFilter_full.png", System.Drawing.Imaging.ImageFormat.Png);
				#endregion
			}
			catch(ArgumentException) {
				PseudoPicturezContextMenu pseudo = new PseudoPicturezContextMenu (true);
				pseudo.Title = Language.I.L [125];
				pseudo.Label1 = string.Empty; // Language.I.L [125];
				pseudo.Label2 = Language.I.L [126];
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.SetPosition (WindowPosition.CenterAlways);
				// pseudo.Show ();
				this.DestroyAll ();
				repeatTimeoutPreprocessing = false;
			}

			FireFilterEvent (st.ResultBitmap);
			this.DestroyAll ();
		}

		protected void OnBtnReleaseEvent (object o, ButtonReleaseEventArgs args)
		{

			timeoutSw.Stop ();
			repeatTimeoutStopwatch = false;

			if (repeatTimeoutPreprocessing)
				return;

			repeatTimeoutPreprocessing = true;
			GLib.Timeout.Add(Constants.TIMEOUT_STITCH_PROCESS_PREVIEW, timeoutHandlerPreprocessing);
		}

		protected void OnBtnPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeoutStopwatch)
				return;

			if (o == btn01LeftPlus) {
				pointerLabel = lb01Left;
				incrementValue = true;
			}
			else if (o == btn01LeftMinus) {
				pointerLabel = lb01Left;
				incrementValue = false;
			}
			else if (o == btn01RightPlus) {
				pointerLabel = lb01Right;
				incrementValue = true;
			}
			else if (o == btn01RightMinus) {
				pointerLabel = lb01Right;
				incrementValue = false;
			}
			else if (o == btn01TopPlus) {
				pointerLabel = lb01Top;
				incrementValue = true;
			}
			else if (o == btn01TopMinus) {
				pointerLabel = lb01Top;
				incrementValue = false;
			}
			else if (o == btn01BottomPlus) {
				pointerLabel = lb01Bottom;
				incrementValue = true;
			}
			else if (o == btn01BottomMinus) {
				pointerLabel = lb01Bottom;
				incrementValue = false;
			}
			else if (o == btn02LeftPlus) {
				pointerLabel = lb02Left;
				incrementValue = true;
			}
			else if (o == btn02LeftMinus) {
				pointerLabel = lb02Left;
				incrementValue = false;
			}
			else if (o == btn02RightPlus) {
				pointerLabel = lb02Right;
				incrementValue = true;
			}
			else if (o == btn02RightMinus) {
				pointerLabel = lb02Right;
				incrementValue = false;
			}
			else if (o == btn02TopPlus) {
				pointerLabel = lb02Top;
				incrementValue = true;
			}
			else if (o == btn02TopMinus) {
				pointerLabel = lb02Top;
				incrementValue = false;
			}
			else if (o == btn02BottomPlus) {
				pointerLabel = lb02Bottom;
				incrementValue = true;
			}
			else if (o == btn02BottomMinus) {
				pointerLabel = lb02Bottom;
				incrementValue = false;
			}


			timeoutSw.Restart ();
			repeatTimeoutStopwatch = true;
			SetImagePaddingAndLabel ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(SetImagePaddingAndLabelByTimeoutHandler));
		}

		#endregion gui events
	}
}

