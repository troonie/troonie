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
		/// <summary> Padding distance between border of ViewerImagePanel and the image itself. </summary>
		private const int padding = 10;
//		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		private static double[] RedColor = new double[4]{ 1, 0, 0, 1 };

		private EventBox eb;
		private DrawingArea da;
		private int maxWidth, maxHeight, smallWidthAndHeight, fontSize;
		private double translateX, translateY, scale;
		private bool isEntered, isPressedIn, isDoubleClicked, firstClick, saveThumbnailsPersistent;
		private Stopwatch sw_doubleClick;
		private string thumbDirectory, thumbSmallName, relativeImageName;
		private Gdk.Pixbuf pix;
		private CairoColor workingColor;

		public TagsData TagsData;
		public OnIsPressedInChangedEventHandler OnIsPressedInChanged;
		public OnIsPressedInChangedEventHandler OnDoubleClicked;

		#region Public properties

		public int ID  { get; private set; }
		public bool IsVideo  { get; private set; }
		public bool IsPressedIn
		{ 
			get
			{ 
				return isPressedIn;
			} 
			set
			{ 
				isPressedIn = value;

				//fire the event OnIsPressedInChanged
				if (OnIsPressedInChanged != null) //is there a EventHandler?
				{
					OnIsPressedInChanged.Invoke(this); //calls its EventHandler                
				} //if not, ignore

				if (value) {
					workingColor = Cc.BtnPressedin.I.Down;
				} else {
					workingColor = Cc.Instance.C_GRID;
				}

				da.QueueDraw ();
			}
		}
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
					//fire the event OnDoubleClicked
					if (OnDoubleClicked!= null) //is there a EventHandler?
					{
						OnDoubleClicked.Invoke(this); //calls its EventHandler                
					} //if not, ignore

					W = maxWidth;
					H = maxHeight;

					SetFullImage( );
				} else {
					W = smallWidthAndHeight + padding;
					H = smallWidthAndHeight + padding;

					SetThumbnailImage ();
				}
			}
		}
		public string OriginalImageFullName { get; private set; }
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

		#endregion Public properties


		public ViewerImagePanel (int id, bool isVideo, string originalImageFullName, int smallWidthAndHeight, int maxWidth, int maxHeight, bool saveThumbnailsPersistent = true)
		{
			ID = id;
			this.IsVideo = isVideo;
			this.smallWidthAndHeight = smallWidthAndHeight;
			this.maxWidth = maxWidth;
			this.maxHeight = maxHeight;
			fontSize = (int)Math.Max(12, (Screen.Height * 12 / 1200.0));

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
			relativeImageName = originalImageFullName.Substring(originalImageFullName.LastIndexOf(IOPath.DirectorySeparatorChar) + 1);
			string l_relativeImageName = relativeImageName.Substring(0, relativeImageName.LastIndexOf('.'));
			thumbSmallName = l_relativeImageName + smallWidthAndHeight.ToString() + 
				Constants.Extensions[TroonieImageFormat.JPEG24].Item1;

			OriginalImageFullName = originalImageFullName;
			TagsData = IsVideo ? VideoTagHelper.GetTagsData (OriginalImageFullName) : 
								 ImageTagHelper.GetTagsData (OriginalImageFullName);

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

			W = smallWidthAndHeight + padding;
			H = smallWidthAndHeight + padding;

			workingColor = Cc.Instance.C_GRID;

			QueueDraw();

			Thread thread = new Thread(SetThumbnailImage);
			thread.IsBackground = true;
			thread.Start();

		}			
			
		private void SetThumbnailImage()
		{
			if (IsVideo) {
				return;
			}

			if (!File.Exists (thumbDirectory + thumbSmallName)) {
				
				BitmapWithTag bt = new BitmapWithTag (OriginalImageFullName, true);
				Config c = new Config ();
				c.BiggestLength = smallWidthAndHeight;
				c.FileOverwriting = false;
				c.Path = thumbDirectory;
				c.JpgQuality = 80;
				c.Format = TroonieImageFormat.JPEG24;
				c.ResizeVersion = ResizeVersion.BiggestLength;

				// TODO: Catch, what should be done, if success==false
				bool successSmall = bt.Save (c, thumbSmallName, false);

				bt.Dispose ();
			}

			pix = new Gdk.Pixbuf(thumbDirectory + thumbSmallName);
//			pix = new Gdk.Pixbuf(originalImageFullName);

			if (pix.Width > pix.Height) {
				double ratio = pix.Height / (double)pix.Width;
				translateX = 0;
				translateY = (smallWidthAndHeight - smallWidthAndHeight * ratio) / 2.0;
				scale = smallWidthAndHeight / (double)pix.Width;

			} else {
				double ratio = pix.Width / (double)pix.Height;
				translateY = 0;
				translateX = (smallWidthAndHeight - smallWidthAndHeight * ratio) / 2.0;
				scale = smallWidthAndHeight / (double)pix.Height;
			}

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

		private void SetFullImage()
		{
			if (IsVideo) {
				return;
			}

			pix = new Gdk.Pixbuf(OriginalImageFullName);
			double sW = maxWidth / (double)pix.Width;
			double sH = maxHeight / (double)pix.Height;
			scale = Math.Min (sW, sH);

			if (sW > sH) {
				translateX = (maxWidth - pix.Width * scale) / 2.0;
				translateY = 0;
			}
			else {
				translateY = (maxHeight - pix.Height * scale) / 2.0;
				translateX = 0;
			}
		}

		#region protected events

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
			} else if (IsVideo) {
				cr.Save();
				//			cr.SetSourceRGB(RedColor[0], RedColor[1], RedColor[2]);
				cr.SetSourceRGB(0.4,0,0.8);
				cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
				cr.SetFontSize(fontSize * 2);

//				cr.MoveTo(6, 20); // links oben
				cr.MoveTo((W - (fontSize * 2)) / 2.6, (H - padding) / 2.0); 
				cr.ShowText ("VIDEO");

				cr.Restore();
			}

			cr.Save();
//			cr.SetSourceRGB(RedColor[0], RedColor[1], RedColor[2]);
			cr.SetSourceRGB(0,0,0);
			cr.SelectFontFace("Arial", FontSlant.Normal, FontWeight.Bold);
			cr.SetFontSize(fontSize);

			cr.MoveTo(6, 20); // links oben
			cr.ShowText (relativeImageName);

			cr.SetSourceRGB(RedColor[0], RedColor[1], RedColor[2]);
			cr.MoveTo(W /*303*/ - fontSize, 20); // rechts oben

			if (TagsData.Rating.HasValue && TagsData.Rating.Value != 0) {
				cr.ShowText (TagsData.Rating.Value.ToString ());
			}

			cr.SetSourceRGB(0,0,1);
			cr.MoveTo(6, H - padding /*300*/); // links unten
			// cr.MoveTo(303 - fontSize / 2, 303); // rechts unten
			if (TagsData.Keywords != null && TagsData.Keywords.Count != 0) {
				string all = "";
				foreach (var s in TagsData.Keywords) {
					all += s + ", ";
				}
				all = all.Remove(all.Length - 2);

				cr.ShowText (all);
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
				firstClick = true;
				if (sw_doubleClick.ElapsedMilliseconds < Constants.TIME_DOUBLECLICK) {

					IsDoubleClicked = true;
//					//fire the event OnDoubleClicked
//					if (OnDoubleClicked!= null) //is there a EventHandler?
//					{
//						OnDoubleClicked.Invoke(this); //calls its EventHandler                
//					} //if not, ignore

//					ViewerStandaloneImagePanel vsaip = 
//						new ViewerStandaloneImagePanel (OriginalImageFullName, thumbDirectory + thumbBigName);
//					vsaip.Show ();

					return;
				}

			}

			isPressedIn = !isPressedIn;
			workingColor = Cc.Instance.Cairo_Orange;
			da.QueueDraw ();
		}

		protected void OnEbButtonRelease (object o, ButtonReleaseEventArgs args)
		{
			if (IsDoubleClicked) {
				isPressedIn = true;
			}

			IsPressedIn = isPressedIn;
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

		#endregion protected events


		public override void Destroy ()
		{
			if (pix != null) {
				pix.Dispose ();
				pix = null;
			}				
			//			drawingAreaImage.Destroy();
			base.Destroy ();
		}			
			
	}
}

