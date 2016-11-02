using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;
using Cairo;
using System.Diagnostics;
using Troonie_Lib;
using System.IO;
using System.Threading;
using IOPath = System.IO.Path;
using Cc = Troonie.ColorConverter;
using CairoColor = Cairo.Color;
using Ic = Troonie_Lib.ImageConverter;

namespace Troonie
{
	/// <summary>Event handler for changing cursor position on image panel.</summary>
	public delegate void OnCursorPosChangedViewerImagePanelEventHandler(int x, int y);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class ViewerImagePanel2 : Bin
	{	
		/// <summary> Padding distance between border of ViewerImagePanel2 and the image itself. </summary>
		private const int padding = 10;
		private const double transparency = 0.8; 
		private const int BiggestLengthBig = 800;
		public const int BiggestLengthSmall = 300;


//		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		private static double[] RedColor = new double[4]{ 255/255.0, 0/255.0, 0, 255/255.0 };

		private EventBox eb;
		private DrawingArea da;
		private double translateX, translateY;
//		private int biggestLength;
		private bool isEntered, firstClick, saveThumbnailsPersistent;
		private Stopwatch sw_doubleClick;

		private Cairo.ImageSurface surface;
		private CairoColor workingColor;

		private string thumbSmallName, thumbBigName, thumbDirectory;
		public bool IsPressedin { get; private set;	}
		public string OriginalImageFullName { get; private set; }
		public TagsData TagsData; // { get; private set; }

//		public string SmallThumbnailPath { get; private set; }
//		public string BigThumbnailPath { get; private set; }


		/// <summary> Shortcut for <see cref="WidthRequest"/> as well as <see cref="drawingAreaImage.WidthRequest"/>.</summary>
		public int W 
		{ 
			get 
			{ 
				return WidthRequest; 
			}
			set
			{
				WidthRequest = value;
				da.WidthRequest= value;
			}
		}

		/// <summary> Shortcut for <see cref="HeightRequest"/> as well as <see cref="drawingAreaImage.HeightRequest"/>.</summary>
		public int H 
		{ 
			get 
			{ 
				return HeightRequest; 
			}
			set
			{
				HeightRequest = value;
				da.HeightRequest= value;
			}
		}

		/// <summary>
		/// Factor for multiplying cursor's X-coordinate at image panel 
		/// to get correct image pixel coordinate.
		/// </summary>
		public float ScaleCursorX { get; set; }
		/// <summary>
		/// Factor for multiplying cursor's Y-coordinate at image panel 
		/// to get correct image pixel coordinate.
		/// </summary>
		public float ScaleCursorY { get; set; }
//		public string SurfaceFileName { get; set; }
		/// <summary>Handles the event at the client.</summary>
		public OnCursorPosChangedSimpleImagePanelEventHandler OnCursorPosChanged;

		public ViewerImagePanel2 (string originalImageFullName, bool saveThumbnailsPersistent)
		{
			const string thumbDirName = "_TroonieThumbs";
			this.saveThumbnailsPersistent = saveThumbnailsPersistent;

			if (saveThumbnailsPersistent) {
				thumbDirectory = IOPath.GetDirectoryName (originalImageFullName) + IOPath.DirectorySeparatorChar + 
					thumbDirName + IOPath.DirectorySeparatorChar;
			} else {
				thumbDirectory = Constants.I.EXEPATH + IOPath.DirectorySeparatorChar + 
					thumbDirName + IOPath.DirectorySeparatorChar;
			}
				
			Directory.CreateDirectory (thumbDirectory);
			string relativeImageName = originalImageFullName.Substring(originalImageFullName.LastIndexOf(IOPath.DirectorySeparatorChar) + 1);
			relativeImageName = relativeImageName.Substring(0, relativeImageName.LastIndexOf('.'));
			thumbSmallName = relativeImageName + BiggestLengthSmall.ToString() + 
				Constants.Extensions[TroonieImageFormat.PNG24].Item1;
			thumbBigName = relativeImageName + BiggestLengthBig.ToString() + 
				Constants.Extensions[TroonieImageFormat.PNG24].Item1;

			OriginalImageFullName = originalImageFullName;
			TagsData = ImageTagHelper.GetTagsData (OriginalImageFullName);

			firstClick = true;
			sw_doubleClick = new Stopwatch();

			Stetic.Gui.Initialize (this);
			Stetic.BinContainer.Attach (this);

			Events = ((Gdk.EventMask)(1024));

			eb = new EventBox ();
			da = new DrawingArea ();
			da.CanFocus = true;
			da.Events = ((Gdk.EventMask)(772)); 

			eb.Add (da);
			Add (eb);

			da.ExposeEvent += OnDaExpose;
			da.MotionNotifyEvent += OnDaMotionNotify;

			eb.ButtonPressEvent += OnEbButtonPress;
			eb.ButtonReleaseEvent += OnEbButtonRelease;
			eb.EnterNotifyEvent += OnEbEnterNotify;
			eb.LeaveNotifyEvent += OnEbLeaveNotify;

//			ModifyBg(StateType.Normal, Cc.Instance.Green);

//			if (surface != null) {
//				surface.Dispose ();
//				surface = null;
//			}
			// surface = new Cairo.ImageSurface (SmallThumbnailPath);
			surface = new Cairo.ImageSurface (Format.A8, 1, 1);

//			biggestLength = Math.Max (surface.Width, surface.Height);
//			translateX = Math.Max (0, surface.Height - surface.Width) / 2.0;
//			translateY = Math.Max (0, surface.Width - surface.Height) / 2.0;
//
//			translateX += padding / 2;
//			translateY += padding / 2;

			W = BiggestLengthSmall + padding;
			H = BiggestLengthSmall + padding;

			workingColor = Cc.Instance.C_GRID;


			Thread thread = new Thread(SetThumbnailImage);
			thread.IsBackground = true;
			thread.Start();

			//			thread.ThreadState = System.Threading.ThreadState.Sopped;
//			GLib.Idle.Add(new GLib.IdleHandler(SetThumbnailImage));
		}

		public override void Destroy ()
		{
			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
			//			drawingAreaImage.Destroy();
			base.Destroy ();
		}			

		public void SetPressedIn (bool p_IsPressedin)
		{
			IsPressedin = p_IsPressedin;
			if (IsPressedin) {
				workingColor = Cc.BtnPressedin.I.Down;
			} else {
				workingColor = Cc.Instance.C_GRID;
			}

			da.QueueDraw ();			
		}

//		public void SetRating(uint rating)
//		{
//			if (IsPressedin) {
////				ImageTagHelper.SetAndSaveTag (vip.OriginalImageFullName, Tags.Rating, rating);
//				//					vip.Rating = rating;
//				tagsData.Rating = rating;
//			}
//		}

		private void SetThumbnailImage()
		{
			if (!File.Exists (thumbDirectory + thumbSmallName)) {
				
				BitmapWithTag bt = new BitmapWithTag (OriginalImageFullName, true);
				Config c = new Config ();
				c.BiggestLength = BiggestLengthSmall;
				c.FileOverwriting = false;
				c.Path = thumbDirectory;
				c.Format = TroonieImageFormat.PNG24;
				c.ResizeVersion = ResizeVersion.BiggestLength;

				// TODO: Catch, what should be happen, when success==false
				bool successSmall = bt.Save (c, thumbSmallName, false);

				if (!File.Exists (thumbDirectory + thumbBigName)) {
					bt = new BitmapWithTag (OriginalImageFullName, true);
					c.BiggestLength = BiggestLengthBig;
					bool successBig = bt.Save (c, thumbBigName, false);

				}
				bt.Dispose ();

//				Bitmap b = Bitmap.FromFile (OriginalImageFullName, true) as Bitmap;
//				int w = b.Width;
//				int h = b.Height;
//				Ic.CalcBiggerSideLength (BiggestLengthBig, ref w, ref h);
//
//				Bitmap dest;
//				Ic.ScaleAndCut (b, 
//					out dest,
//					0,
//					0,
//					w,
//					h,
//					ConvertMode.NoStretchForge,
//					true /* config.HighQuality */);
//
//				dest.Save (thumbDirectory + thumbBigName, ImageFormat.Png);
			}


			surface.Dispose ();
			surface = new Cairo.ImageSurface (thumbDirectory + thumbSmallName);

			translateX = Math.Max (0, surface.Height - surface.Width) / 2.0;
			translateY = Math.Max (0, surface.Width - surface.Height) / 2.0;

			translateX += padding / 2;
			translateY += padding / 2;



			// work around by GLib timeout to fix the GTK#-resizing bug
			GLib.TimeoutHandler timeoutHandler = () => {
				QueueDraw();
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(100, timeoutHandler);

//			return false;
		}

		#region GLib.Idle



		#endregion

		#region DrawingAreaImage events

		// todo make static?
		protected void OnDaExpose (object o, ExposeEventArgs args)
		{
//			DrawingArea drawingArea = obj as DrawingArea;
//
//			int width = drawingArea.Allocation.Width;
//			int height = drawingArea.Allocation.Height;
//
//			Cairo.Context cr =  Gdk.CairoHelper.Create(drawingArea.GdkWindow);


//			DrawingArea area = (DrawingArea) o;
			Cairo.Context cr =  Gdk.CairoHelper.Create(da.GdkWindow);

			cr.Save();
			cr.Rectangle (0, 0, W, H);

			if (isEntered)
				cr.SetSourceRGB(Cc.Instance.Cairo_Orange.R, Cc.Instance.Cairo_Orange.G, Cc.Instance.Cairo_Orange.B);
			else
				cr.SetSourceRGB(Cc.BtnNormal.I.Top.R, Cc.BtnNormal.I.Top.G, Cc.BtnNormal.I.Top.B);
//				cr.SetSourceRGB(Cc.Instance.Cairo_Blue.R, Cc.Instance.Cairo_Blue.G, Cc.Instance.Cairo_Blue.B);
						
			cr.Fill ();	
			cr.Restore();

			cr.Save();
			cr.Rectangle (padding / 2, padding / 2, W - padding, H - padding);

			cr.SetSourceRGB(workingColor.R, workingColor.G, workingColor.B);

			cr.Fill ();	
			cr.Restore();


			cr.Save();
			cr.Translate(translateX, translateY);
			//			// invert angle here, because RotateBilinear filter works counter clockwise
			//			cr.Rotate (-Angle*Math.PI/180);
			//			cr.Scale (ScaleForRotation, ScaleForRotation);
			//			cr.Translate(-W / 2.0, -H / 2.0);
			cr.SetSourceSurface(surface, 0, 0);
			cr.Paint();
			cr.Restore();

			cr.Save();
			cr.SetSourceRGB(RedColor[0], RedColor[1], RedColor[2]);
			cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
			cr.SetFontSize(20);

			cr.MoveTo(1, 15);
			if (TagsData.Rating.HasValue && TagsData.Rating.Value != 0) {
				cr.ShowText (TagsData.Rating.Value.ToString ());
			}

//			cr.MoveTo(1, 30);
//			cr.ShowText("2");
//
//			cr.MoveTo(1, 45);
//			cr.ShowText("3");

			cr.Restore();

			((IDisposable) cr.GetTarget()).Dispose();                                      
			((IDisposable) cr).Dispose();
		}

		protected void OnDaMotionNotify (object o, MotionNotifyEventArgs args)
		{
			//fire the event now
			if (this.OnCursorPosChanged != null) //is there a EventHandler?
			{
				int x = (int)Math.Round(args.Event.X * ScaleCursorX);
				int y = (int)Math.Round(args.Event.Y * ScaleCursorY);
				this.OnCursorPosChanged.Invoke(x, y); //calls its EventHandler                
			} //if not, ignore
		}

		protected void OnEbButtonPress (object o, ButtonPressEventArgs args)
		{
			if (firstClick) {
				sw_doubleClick.Restart ();	
				firstClick = false;
			} else {
				sw_doubleClick.Stop ();
				if (sw_doubleClick.ElapsedMilliseconds < Constants.TIME_DOUBLECLICK) {
					ViewerStandaloneImagePanel vsaip = 
						new ViewerStandaloneImagePanel (OriginalImageFullName, thumbDirectory + thumbBigName);
					vsaip.Show ();
				}
				firstClick = true;
			}

			IsPressedin = !IsPressedin;
			workingColor = Cc.Instance.Cairo_Orange;
			da.QueueDraw ();
		}

		protected void OnEbButtonRelease (object o, ButtonReleaseEventArgs args)
		{
			SetPressedIn (IsPressedin);
		}

		public void OnEbEnterNotify(object sender, EnterNotifyEventArgs a)
		{       
			isEntered = true;
			da.QueueDraw ();
		}

		protected void OnEbLeaveNotify(object sender, LeaveNotifyEventArgs a)
		{      
			isEntered = false;
			da.QueueDraw ();
		}

		#endregion DrawingAreaImage events
	}
}

