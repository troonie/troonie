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
	/// <summary>Event handler for changing property 'IsPressedIn'.</summary>
	public delegate void OnIsPressedInChangedEventHandler(ViewerImagePanel viewerImagePanel);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class ViewerImagePanel : Bin
	{	
		public const int BiggestLengthSmall = 300;

		private const double transparency = 0.8; 
		private const int BiggestLengthBig = 800;
		/// <summary> Padding distance between border of ViewerImagePanel2 and the image itself. </summary>
		private const int padding = 10;

//		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		private static double[] RedColor = new double[4]{ 255/255.0, 0/255.0, 0, 255/255.0 };

		private EventBox eb;
		private DrawingArea da;
		int maxWidth, maxHeight;
		private double translateX, translateY, scale;
//		private int biggestLength;
		private bool isEntered, isDoubleClicked, firstClick, saveThumbnailsPersistent;
//		/// <summary> DO NOT USE. Use instead its poperty <paramref name="IsPressedin"/>. </summary>
//		private bool isPressedin;
		private Stopwatch sw_doubleClick;
		private string thumbDirectory, thumbSmallName;

//		private Cairo.ImageSurface surface;
		private Gdk.Pixbuf pix;
		private CairoColor workingColor;

		public int ID  { get; private set; }
		public bool IsPressedin  { get; private set; }
		public bool IsDoubleClicked 
		{ 
			get
			{ 
				return isDoubleClicked;
			} 
			set
			{ 
				isDoubleClicked = value;

				if (value) {
					W = maxWidth;
					H = maxHeight;

					SetFullImage( );
//					SetPixbufImageAndTransformationValues(OriginalImageFullName, Math.Max(W,));
				} else {
					W = BiggestLengthSmall + padding;
					H = BiggestLengthSmall + padding;

					SetThumbnailImage ();
				}
			}
		}
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

		public OnIsPressedInChangedEventHandler OnIsPressedInChanged;
		public OnIsPressedInChangedEventHandler OnDoubleClicked;

		public ViewerImagePanel (int id, string originalImageFullName, int maxWidth, int maxHeight, bool saveThumbnailsPersistent = true)
		{
			ID = id;
			this.maxWidth = maxWidth;
			this.maxHeight = maxHeight;

			const string thumbDirName = "_TroonieThumbs";
			this.saveThumbnailsPersistent = saveThumbnailsPersistent;


			if (saveThumbnailsPersistent) {
				thumbDirectory = IOPath.GetDirectoryName (originalImageFullName) + IOPath.DirectorySeparatorChar + 
					thumbDirName + IOPath.DirectorySeparatorChar;
			} else {
				thumbDirectory = Constants.I.EXEPATH + IOPath.DirectorySeparatorChar + 
					thumbDirName + IOPath.DirectorySeparatorChar;
			}
			thumbDirectory = IOPath.GetDirectoryName (originalImageFullName) + IOPath.DirectorySeparatorChar + 
									thumbDirName + IOPath.DirectorySeparatorChar;
				
			Directory.CreateDirectory (thumbDirectory);
			string relativeImageName = originalImageFullName.Substring(originalImageFullName.LastIndexOf(IOPath.DirectorySeparatorChar) + 1);
			relativeImageName = relativeImageName.Substring(0, relativeImageName.LastIndexOf('.'));
			thumbSmallName = relativeImageName + BiggestLengthSmall.ToString() + 
				Constants.Extensions[TroonieImageFormat.PNG24].Item1;
//			thumbBigName = relativeImageName + BiggestLengthBig.ToString() + 
//				Constants.Extensions[TroonieImageFormat.PNG24].Item1;

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
//			da.MotionNotifyEvent += OnDaMotionNotify;

			eb.ButtonPressEvent += OnEbButtonPress;
			eb.ButtonReleaseEvent += OnEbButtonRelease;
			eb.EnterNotifyEvent += OnEbEnterNotify;
			eb.LeaveNotifyEvent += OnEbLeaveNotify;

//			pix = new Gdk.Pixbuf(originalImageFullName);
//
//			if (pix.Width > pix.Height) {
//				double ratio = pix.Height / (double)pix.Width;
//				translateX = 1;
//				translateY = (BiggestLengthSmall - BiggestLengthSmall * ratio) / 2.0;
//				scale = BiggestLengthSmall / (double)pix.Width;
//
//			} else {
//				double ratio = pix.Width / (double)pix.Height;
//				translateY = 1;
//				translateX = (BiggestLengthSmall - BiggestLengthSmall * ratio) / 2.0;
//				scale = BiggestLengthSmall / (double)pix.Height;
//			}
//
//			translateX += Padding / 2;
//			translateY += Padding / 2;

			W = BiggestLengthSmall + padding;
			H = BiggestLengthSmall + padding;

			workingColor = Cc.Instance.C_GRID;

			QueueDraw();

			Thread thread = new Thread(SetThumbnailImage);
			thread.IsBackground = true;
			thread.Start();

		}

		public override void Destroy ()
		{
			if (pix != null) {
				pix.Dispose ();
				pix = null;
			}				
			//			drawingAreaImage.Destroy();
			base.Destroy ();
		}			

		public void SetPressedIn (bool p_IsPressedin)
		{
			IsPressedin = p_IsPressedin;

			//fire the event OnIsPressedInChanged
			if (OnIsPressedInChanged != null) //is there a EventHandler?
			{
				OnIsPressedInChanged.Invoke(this); //calls its EventHandler                
			} //if not, ignore

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

				// TODO: Catch, what should be done, when success==false
				bool successSmall = bt.Save (c, thumbSmallName, false);

				bt.Dispose ();
			}

			pix = new Gdk.Pixbuf(thumbDirectory + thumbSmallName);
//			pix = new Gdk.Pixbuf(originalImageFullName);

			if (pix.Width > pix.Height) {
				double ratio = pix.Height / (double)pix.Width;
				translateX = 0;
				translateY = (BiggestLengthSmall - BiggestLengthSmall * ratio) / 2.0;
				scale = BiggestLengthSmall / (double)pix.Width;

			} else {
				double ratio = pix.Width / (double)pix.Height;
				translateY = 0;
				translateX = (BiggestLengthSmall - BiggestLengthSmall * ratio) / 2.0;
				scale = BiggestLengthSmall / (double)pix.Height;
			}

			translateX += padding / 2;
			translateY += padding / 2;
//			SetPixbufImageAndTransformationValues(thumbDirectory + thumbSmallName, BiggestLengthSmall);

			// work around by GLib timeout to fix the GTK#-resizing bug
			GLib.TimeoutHandler timeoutHandler = () => {
				QueueDraw();
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(100, timeoutHandler);

//			return false;
		}

		private void SetFullImage()
		{
			pix = new Gdk.Pixbuf(OriginalImageFullName);
			//			pix = new Gdk.Pixbuf(originalImageFullName);

			double tW = maxWidth / (double)pix.Width;
			double tH = maxHeight / (double)pix.Height;
//
//			if (tW > tH) {
//				double ratio = tH / tW;
//				translateX = 0;
//				translateY = (tW - tW * ratio) / 2.0;
////				scale = BiggestLengthSmall / (double)pix.Width;
//
//			} else {
//				double ratio = pix.Width / (double)pix.Height;
//				translateY = 0;
//				translateX = (BiggestLengthSmall - BiggestLengthSmall * ratio) / 2.0;
//				scale = BiggestLengthSmall / (double)pix.Height;
//			}

			scale = Math.Min (maxWidth / (double)pix.Width, maxHeight / (double)pix.Height);

			if (tW > tH) {
				translateX = (maxWidth - pix.Width * scale) / 2.0;
				translateY = 0;
			}
			else {
				translateY = (maxHeight - pix.Height * scale) / 2.0;
				translateX = 0;
			}
//			if (maxWidth > maxHeight) {
////			if (pix.Width > pix.Height) {
//				scale = maxWidth / (double)pix.Width;
//
//			} else {
//				scale = maxHeight / (double)pix.Height;
//			}

//			translateX = 0; // + padding / 2;
//			translateY = 0; // + padding / 2;
		}

		#region DrawingAreaImage events

		// todo make static?
		protected void OnDaExpose (object o, ExposeEventArgs args)
		{
			Cairo.Context cr =  Gdk.CairoHelper.Create(da.GdkWindow);

			cr.Save();
			cr.Rectangle (0, 0, W, H);

			if (isEntered)
				cr.SetSourceRGB(Cc.Instance.Cairo_Orange.R, Cc.Instance.Cairo_Orange.G, Cc.Instance.Cairo_Orange.B);
			else
				cr.SetSourceRGB(Cc.BtnNormal.I.Top.R, Cc.BtnNormal.I.Top.G, Cc.BtnNormal.I.Top.B);
						
			cr.Fill ();	
			cr.Restore();

			cr.Save();
			cr.Rectangle (padding / 2, padding / 2, W - padding, H - padding);

			cr.SetSourceRGB(workingColor.R, workingColor.G, workingColor.B);

			cr.Fill ();	
			cr.Restore();

			if (pix != null) {
				cr.Save ();
				cr.Translate (translateX, translateY);
				cr.Scale (scale, scale);
				Gdk.CairoHelper.SetSourcePixbuf (cr, pix, 0, 0);
//			cr.SetSourceSurface(surface, 0, 0);
				cr.Paint ();
				cr.Restore ();
			}

			cr.Save();
			cr.SetSourceRGB(RedColor[0], RedColor[1], RedColor[2]);
			cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
			cr.SetFontSize(20);

			cr.MoveTo(1, 15);
			if (TagsData.Rating.HasValue && TagsData.Rating.Value != 0) {
				cr.ShowText (TagsData.Rating.Value.ToString ());
			}

			cr.Restore();

			((IDisposable) cr.GetTarget()).Dispose();                                      
			((IDisposable) cr).Dispose();
		}
			
		protected void OnEbButtonPress (object o, ButtonPressEventArgs args)
		{
			if (firstClick) {
				sw_doubleClick.Restart ();	
				firstClick = false;
			} else {
				sw_doubleClick.Stop ();
				if (sw_doubleClick.ElapsedMilliseconds < Constants.TIME_DOUBLECLICK) {

					IsDoubleClicked = true;
					//fire the event OnDoubleClicked
					if (OnDoubleClicked!= null) //is there a EventHandler?
					{
						OnDoubleClicked.Invoke(this); //calls its EventHandler                
					} //if not, ignore

//					ViewerStandaloneImagePanel vsaip = 
//						new ViewerStandaloneImagePanel (OriginalImageFullName, thumbDirectory + thumbBigName);
//					vsaip.Show ();
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

