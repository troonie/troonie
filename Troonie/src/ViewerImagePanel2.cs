using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;
using Cairo;
using Cc = Troonie.ColorConverter;
using CairoColor = Cairo.Color;

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

		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		private static double[] RedColor = new double[4]{ 255/255.0, 0/255.0, 0, 255/255.0 };

		private EventBox eb;
		private DrawingArea da;
		private double translateX, translateY;
		private int biggestLength;
		private bool isEntered, IsPressedin;

		private Cairo.ImageSurface surface;
		private CairoColor workingColor;



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
		public string SurfaceFileName { get; set; }
		/// <summary>Handles the event at the client.</summary>
		public OnCursorPosChangedSimpleImagePanelEventHandler OnCursorPosChanged;

		public ViewerImagePanel2 ()
		{
			//			this.Build ();
			Stetic.Gui.Initialize (this);
			Stetic.BinContainer.Attach (this);

			Events = ((Gdk.EventMask)(1024));

			eb = new EventBox ();
//			this.fixed1.Name = "fixed1";
//			this.fixed1.HasWindow = false;
			// Container child fixed1.Gtk.Fixed+FixedChild
			da = new DrawingArea ();
			da.CanFocus = true;
			da.Events = ((Gdk.EventMask)(772)); 
			eb.Add (da);

			Add (eb);
//			if (Child != null) {
//				Child.ShowAll ();
//			}
			da.ExposeEvent += OnDaExpose;
			da.MotionNotifyEvent += OnDaMotionNotify;
			eb.ButtonPressEvent += OnDaButtonPress;
			eb.ButtonReleaseEvent += OnDaButtonRelease;

			eb.EnterNotifyEvent += OnEbEnterNotify;
			eb.LeaveNotifyEvent += OnEbLeaveNotify;

//			ModifyBg(StateType.Normal, Cc.Instance.Green);
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

		public void Initialize()
		{
			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
			surface = new Cairo.ImageSurface (SurfaceFileName);

			biggestLength = Math.Max (surface.Width, surface.Height);
			translateX = Math.Max (0, surface.Height - surface.Width) / 2.0;
			translateY = Math.Max (0, surface.Width - surface.Height) / 2.0;

			translateX += padding / 2;
			translateY += padding / 2;

			W = biggestLength + padding;
			H = biggestLength + padding;

			workingColor = Cc.Instance.C_GRID;

			QueueDraw();

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
			cr.SetFontSize(15);

			cr.MoveTo(1, 15);
			cr.ShowText("1");

			cr.MoveTo(1, 30);
			cr.ShowText("2");

			cr.MoveTo(1, 45);
			cr.ShowText("3");

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

		protected void OnDaButtonPress (object o, ButtonPressEventArgs args)
		{
			IsPressedin = !IsPressedin;
			workingColor = Cc.Instance.Cairo_Orange;
			da.QueueDraw ();
		}

		protected void OnDaButtonRelease (object o, ButtonReleaseEventArgs args)
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

