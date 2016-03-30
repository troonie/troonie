using System;

namespace Picturez
{
	/// <summary>Event handler for changing slider values (XGlobal or YGlobal).</summary>
	public delegate void OnSliderChangedValueEventHandler(Slider.Types t, float v);

	[System.ComponentModel.ToolboxItem (true)]
	public class Slider : Gtk.DrawingArea
	{	
		#region Constants
		private static double[] NormalColor = new double[3]{ 0, /*NICHT FF sondern 80*/0.7, 0 };
		private static double[] EnteredColor = new double[3]{ 1, 20/255.0, 20/255.0 };
		public const int LINEWIDTH = 3;
		#endregion Constants

		public enum Types
		{
			Left,
			Right,
			Top,
			Bottom
		}

		private int w, h;
		float xGlobal, yGlobal;
		private bool isEntered;
		private double[] color = NormalColor;

		/// <summary>Handles the event at the client.</summary>
		public OnSliderChangedValueEventHandler OnSliderChangedValue;
		public Slider Partner { get; set; }
		public Types TYPE { get; private set; }
		public bool IsPressed { get; set; }
		public bool IsEntered {
			get {
				return isEntered;
			}
			set {
				isEntered = value;
				color = value ? EnteredColor : NormalColor;
				// redraw slider
				QueueDraw();
			}
		}
		public float XGlobal {
			get {
				return xGlobal;
			}
			set {
				if (xGlobal != value) {
					xGlobal = value;
					FireSliderChangedValueEvent (value);
				} 
			}
		}
		public float YGlobal {
			get {
				return yGlobal;
			}
			set {
				if (yGlobal != value) {
					yGlobal = value;
					FireSliderChangedValueEvent (value);
				}
			}
		}

		public float DistXToPartner {	get { return Math.Abs(XGlobal - Partner.XGlobal); }	}
		public float DistYToPartner {	get { return Math.Abs(YGlobal - Partner.YGlobal); }	}

		public Slider (Types t, int xGlobal, int yGlobal, int w, int h)
		{
			this.xGlobal = xGlobal;
			this.yGlobal = yGlobal;
			this.w = w;
			this.h = h;
			TYPE = t;
		}

		/// <summary>Fires the slider changed value event.</summary>
		private void FireSliderChangedValueEvent(float v)
		{
			//fire the event now
			if (this.OnSliderChangedValue != null) //is there a EventHandler?
			{
				this.OnSliderChangedValue.Invoke(this.TYPE, v); //calls its EventHandler                
			}
			else { } //if not, ignore
		}

		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			// Insert drawing code here.
			// DrawingArea area = (DrawingArea) sender;
			Gdk.Window win = ev.Window;
			Gdk.Rectangle area = ev.Area;
			Cairo.Context cc =  Gdk.CairoHelper.Create(win);

			cc.SetSourceRGB (color[0], color[1], color[2]);
			cc.LineWidth = LINEWIDTH;

			cc.Rectangle(0, 0, w, h);
			//cc.LineTo(0, 0 );
			//cc.Rectangle(10, 10, 80, 80);
			cc.StrokePreserve();

//			cc.SetSourceRGB(1, 1, 1);
//			cc.Fill();

			((IDisposable) cc.GetTarget()).Dispose();                                      
			((IDisposable) cc).Dispose();
//
			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}

		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			// Calculate desired size here.
			requisition.Height = h;
			requisition.Width = w;
		}
	}
}

