using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez
{
	/// <summary>Event handler for changing cursor position on image panel.</summary>
	public delegate void OnCursorPosChangedSimpleImagePanelEventHandler(int x, int y);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class SimpleImagePanel : Bin
	{	
		private Fixed fixed1;
		private DrawingArea drawingAreaImage;

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

		public SimpleImagePanel ()
		{
//			this.Build ();

			global::Stetic.Gui.Initialize (this);
			// Widget Picturez.ImagePanel
			global::Stetic.BinContainer.Attach (this);
			this.Events = ((global::Gdk.EventMask)(1024));
			this.Name = "Picturez.ShaderImagePanel";

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

		public override void Dispose ()
		{
			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
			drawingAreaImage.Dispose();
			base.Dispose ();
		}

		public void Initialize()
		{
			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
			surface = new Cairo.ImageSurface (SurfaceFileName);
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

//			cr.Save();
//			cr.Translate(W / 2.0, H / 2.0);
//			// invert angle here, because RotateBilinear filter works counter clockwise
//			cr.Rotate (-Angle*Math.PI/180);
//			cr.Scale (ScaleForRotation, ScaleForRotation);
//			cr.Translate(-W / 2.0, -H / 2.0);
			cr.SetSourceSurface(surface, 0, 0);
			cr.Paint();
//			cr.Restore();

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

