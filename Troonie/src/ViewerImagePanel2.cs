using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;
using Cairo;

namespace Troonie
{
	/// <summary>Event handler for changing cursor position on image panel.</summary>
	public delegate void OnCursorPosChangedViewerImagePanelEventHandler(int x, int y);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class ViewerImagePanel2 : Bin
	{	
		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		private static double[] RedColor = new double[4]{ 255/255.0, 0/255.0, 0, 255/255.0 };

		private Fixed fixed1;
		private DrawingArea drawingAreaImage;
		private double translateX, translateY;

		private Cairo.ImageSurface surface;

		/// <summary> Shortcut for <see cref="ImagePanel.WidthRequest"/>.</summary>
		public int W { get { return this.WidthRequest; } }
		/// <summary> Shortcut for <see cref="ImagePanel.HeightRequest"/>.</summary>
		public int H { get { return this.HeightRequest; } }

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

			global::Stetic.Gui.Initialize (this);
			// Widget Troonie.ImagePanel
			global::Stetic.BinContainer.Attach (this);
			this.Events = ((global::Gdk.EventMask)(1024));
			this.Name = "Troonie.ViewerImagePanel";

			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			// Container child fixed1.Gtk.Fixed+FixedChild
			this.drawingAreaImage = new global::Gtk.DrawingArea ();
			this.drawingAreaImage.CanFocus = true;
			this.drawingAreaImage.Events = ((global::Gdk.EventMask)(772));
			this.drawingAreaImage.Name = "drawingAreaImage";
			this.fixed1.Add (this.drawingAreaImage);
			this.Add (this.fixed1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			//			this.Hide ();
			this.drawingAreaImage.ExposeEvent += new global::Gtk.ExposeEventHandler (this.OnDrawingAreaImageExposeEvent);
			this.drawingAreaImage.MotionNotifyEvent += new global::Gtk.MotionNotifyEventHandler (this.OnDrawingAreaImageMotionNotifyEvent);
			this.drawingAreaImage.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnDrawingAreaImageButtonPressEvent);
			this.drawingAreaImage.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnDrawingAreaImageButtonReleaseEvent);
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
			translateX = Math.Max (0, surface.Height - surface.Width) / 2.0;
			translateY = Math.Max (0, surface.Width - surface.Height) / 2.0;

			drawingAreaImage.WidthRequest = W;
			drawingAreaImage.HeightRequest = H;

			fixed1.QueueDraw();
		}

		#region DrawingAreaImage events

		// todo make static?
		protected void OnDrawingAreaImageExposeEvent (object o, ExposeEventArgs args)
		{
			DrawingArea area = (DrawingArea) o;
			Cairo.Context cr =  Gdk.CairoHelper.Create(area.GdkWindow);

			cr.Save();

			cr.Rectangle (0, 0, W, H);
			cr.SetSourceRGBA (RectColor[0], RectColor[1], RectColor[2], RectColor[3]);
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
			cr.SetFontSize(12);

			cr.MoveTo(0, 10);
			cr.ShowText("1");

			cr.MoveTo(0, 20);
			cr.ShowText("2");

			cr.MoveTo(0, 30);
			cr.ShowText("3");

			cr.Restore();

			((IDisposable) cr.GetTarget()).Dispose();                                      
			((IDisposable) cr).Dispose();
		}

		protected void OnDrawingAreaImageMotionNotifyEvent (object o, MotionNotifyEventArgs args)
		{
			//fire the event now
			if (this.OnCursorPosChanged != null) //is there a EventHandler?
			{
				int x = (int)Math.Round(args.Event.X * ScaleCursorX);
				int y = (int)Math.Round(args.Event.Y * ScaleCursorY);
				this.OnCursorPosChanged.Invoke(x, y); //calls its EventHandler                
			} //if not, ignore
		}

		protected void OnDrawingAreaImageButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			//			isPressed = true;
			//			fixed1.GetPointer(out xAtPressedBtn, out yAtPressedBtn);
		}

		protected void OnDrawingAreaImageButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//			isPressed = false;
		}

		#endregion DrawingAreaImage events
	}
}

