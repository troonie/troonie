using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;
using Cairo;
using Cc = Troonie.ColorConverter;
using CairoColor = Cairo.Color;
using System.Diagnostics;
using Troonie_Lib;
using Gdk;

namespace Troonie
{
//	/// <summary>Event handler for changing cursor position on image panel.</summary>
//	public delegate void OnCursorPosChangedViewerImagePanelEventHandler(int x, int y);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class ViewerStandaloneImagePanel : Gtk.Window
	{	
		private const double startZoom = 2.5;
		private const double minZoom = 0.2;
		private const double scrollZoomSummand = 0.1;

		private static double[] RedColor = new double[4]{ 255/255.0, 0/255.0, 0, 255/255.0 };

		private EventBox eb;
		private DrawingArea da;
//		private double translateX, translateY;
//		private int biggestLength;

		private Cairo.ImageSurface surface;
//		private CairoColor workingColor;
		private double maxZoom;

		public string OriginalPath { get; private set; }
		public string ThumbnailPath { get; private set; }

		public double Zoom { get; set; }

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

		public ViewerStandaloneImagePanel (string originalPath, string thumbnailPath) :	base (Gtk.WindowType.Toplevel)
		{
			this.KeepAbove = true;

			OriginalPath = originalPath;
			ThumbnailPath = thumbnailPath;

			Stetic.Gui.Initialize (this);
			Stetic.BinContainer.Attach (this);

//			Events = Gdk.EventMask.AllEventsMask;
			DeleteEvent += OnDeleteEvent;

			eb = new EventBox ();
			da = new DrawingArea ();
			da.CanFocus = true;
//			da.Events = ((Gdk.EventMask)(772)); 

			eb.Add (da);
			Add (eb);

			da.ExposeEvent += OnDaExpose;
			ButtonReleaseEvent += OnButtonReleaseEvent;
			ScrollEvent += OnScrollEvent;

			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
			surface = new Cairo.ImageSurface (ThumbnailPath);		

			int wx = 20;
			int wy = 20;
			int tW = Screen.Width - 2 * wx;
			int tH = Screen.Height - 2 * wy - 100 /*taskbarHeight*/;

			maxZoom = Math.Min(tW / surface.Width, tH / surface.Height);
			Zoom = Math.Min (startZoom, maxZoom);
			W = (int) (surface.Width * Zoom);
			H = (int) (surface.Height * Zoom);


			Move (wx, wy);
//			Resize (startWidth, startHeight);

			SetGuiColors ();
			SetLanguageToGui ();
//			QueueDraw();

			ShowAll();
		}

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, Cc.Instance.GRID);
//			this.ModifyBg(StateType.Normal, Cc.GRID);
			//			this.tableViewer.ModifyFg(StateType.Normal, colorConverter.GRID);
			//			this.scrolledwindowViewer.ModifyFg(StateType.Normal, colorConverter.GRID);

			//			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			//			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
			//
			//			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
			//			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
			//			lbFrameKey.ModifyFg (StateType.Normal, colorConverter.FONT);
			//			lbFrameContent.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
			//			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
			//			btnOk.Text = Language.I.L[16];
			//			btnOk.Redraw ();
			//
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();
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
			
		protected void OnDaExpose (object o, ExposeEventArgs args)
		{
			Cairo.Context cr =  Gdk.CairoHelper.Create(da.GdkWindow);

			cr.Save();
			cr.Rectangle (0, 0, W, H);
			cr.SetSourceRGB(Cc.Instance.C_GRID.R, Cc.Instance.C_GRID.G, Cc.Instance.C_GRID.B);
			cr.Fill ();	
			cr.Restore();

			cr.Save();
//			cr.Translate(5, 5);
			cr.Scale (Zoom, Zoom);
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


		protected virtual void OnScrollEvent (object o, Gtk.ScrollEventArgs args)
		{			
			switch ( args.Event.Direction )
			{
			case ScrollDirection.Down: 
				Zoom = Math.Max(Zoom - scrollZoomSummand, minZoom);
				break;
			case ScrollDirection.Up: 
				Zoom = Math.Min (Zoom + scrollZoomSummand, maxZoom);
				break;
			}

			W = (int) (surface.Width * Zoom);
			H = (int) (surface.Height * Zoom);
			Resize (W, H);
		} 

		protected virtual void OnButtonReleaseEvent (object o, Gtk.ButtonReleaseEventArgs args)
		{
			switch ( args.Event.Button )
			{
			case 1: /*left button*/
				Console.WriteLine( "Left Mouse button released" );
				break;
			case 2: /*middle button*/
				Console.WriteLine( "Middle Mouse button released" );
				break;
			case 3: /* right button */
				Console.WriteLine( "Right Mouse button released" );
				break;
			}
		} 
	}
}

