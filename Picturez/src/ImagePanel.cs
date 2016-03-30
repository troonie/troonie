using System;
using Gtk;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez
{
	/// <summary>Event handler for changing cursor position on image panel.</summary>
	public delegate void OnCursorPosChangedEventHandler(int x, int y);

	[System.ComponentModel.ToolboxItem (true)]
	public partial class ImagePanel : Bin
	{	
		#region Constants
		public const int MINDISTANCE = 35;
		private static double[] RectColor = new double[4]{ 220/255.0, 220/255.0, 1, 220/255.0 };
		#endregion Constants

		private Cairo.ImageSurface surface;
		/// <summary> Button is pressed NOT on a slider.</summary>
		private bool isPressed;
		private int xAtPressedBtn, yAtPressedBtn;
		private float sliderDiffAfterButtonPressX;
		private float sliderDiffAfterButtonPressY;
		/// <summary> Shortcut for <see cref="ImagePanel.WidthRequest"/>.</summary>
		public int W { get { return this.WidthRequest; } }
		/// <summary> Shortcut for <see cref="ImagePanel.HeightRequest"/>.</summary>
		public int H { get { return this.HeightRequest; } }
		public Slider LeftSlider { get; private set;}
		public Slider RightSlider { get; private set;}
		public Slider TopSlider { get; private set;}
		public Slider BottomSlider { get; private set;}

		double scaleForRotation;
		/// <summary> Scale factor, needed by image rotation. </summary>
		public double ScaleForRotation {
			get {
				return scaleForRotation;
			}
			private set {
				scaleForRotation = value;
			}
		}

		private double angle;
		/// <summary> Rotation angle. </summary>
		public double Angle {
			get {
				return angle;
			}
			set {
				angle = value;
				Size newSize = Picturez_Lib.RotateBilinear.CalculateNewImageSize (Angle, W, H, false);
				double scaleX = (double)W / newSize.Width;
				double scaleY = (double)H / newSize.Height;
				ScaleForRotation = Math.Min (scaleX, scaleY);
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
		public OnCursorPosChangedEventHandler OnCursorPosChanged;

		public ImagePanel ()
		{
			this.Build ();
		}	

		public override void Destroy ()
		{
			DisposeFixedWidgetChildrenAndSurface();
			drawingAreaImage.Destroy ();
			drawingAreaImage.Dispose();
			base.Destroy ();
//			base.Dispose ();
		}

		public void Initialize()
		{
			DisposeFixedWidgetChildrenAndSurface ();
			surface = new Cairo.ImageSurface (SurfaceFileName);
			drawingAreaImage.WidthRequest = W;
			drawingAreaImage.HeightRequest = H;
			Angle = 0;

			AddSliders ();

			fixed1.QueueDraw();
		}

//		public void MoveSliderByButton(Slider p_Slider)
//		{
//			float destValue;
//			float factorX = 1 / ScaleCursorX;
//			float factorY = 1 / ScaleCursorY;
//
//			p_Slider.IsEntered = true;
//			destValue = p_Slider.XGlobal - factorX;
//			if (CheckMoving (p_Slider.TYPE, ref destValue, W)) {
//				MoveSlider (p_Slider, fixed1, destValue);
//
//			}
//		}

		/// <summary>
		/// Moves the slider directly at passed position.
		/// Returns <c>true</c>, if slider was moved, <c>false</c> otherwise.
		/// </summary>
		/// <returns><c>true</c>, if slider was moved, <c>false</c> otherwise.</returns>
		public bool MoveSliderByValue (Slider s, int positionValue)
		{
			float lScale;
			int lWidthOrHeight;
			if (s.TYPE == Slider.Types.Left || s.TYPE == Slider.Types.Right) {
				lScale = ScaleCursorX;
				lWidthOrHeight = W;
			} else {
				lScale = ScaleCursorY;
				lWidthOrHeight = H;
			}

			float destValue = positionValue / lScale;
			bool success;
			if (success = CheckMoving (s.TYPE, ref destValue, lWidthOrHeight)) {
				MoveSlider (s, fixed1, destValue);
			}
			return success;
		}

		// [GLib.ConnectBefore ()] 
		public void MoveSliderByKey (Gdk.Key pKey, int pMoveValue)
		{
			float destValue;
			float factorX = pMoveValue / ScaleCursorX;
			float factorY = pMoveValue / ScaleCursorY;

			switch (pKey) {
			case Gdk.Key.Left:
				if (LeftSlider.IsEntered) {
					destValue = LeftSlider.XGlobal - factorX;
					if (CheckMoving (LeftSlider.TYPE, ref destValue, W)) {
						MoveSlider (LeftSlider, fixed1, destValue);
					}
				} else if (RightSlider.IsEntered) {
					destValue = RightSlider.XGlobal - factorX;
					if (CheckMoving (RightSlider.TYPE, ref destValue, W)) {
						MoveSlider (RightSlider, fixed1, destValue);
					}
				}
				break;			
			case Gdk.Key.Right:
					if (LeftSlider.IsEntered) {
					destValue = LeftSlider.XGlobal + factorX;
						if (CheckMoving (LeftSlider.TYPE, ref destValue, W)) {
							MoveSlider (LeftSlider, fixed1, destValue);
						}
					} 
					else if (RightSlider.IsEntered){
					destValue = RightSlider.XGlobal + factorX;
						if (CheckMoving (RightSlider.TYPE, ref destValue, W)) {
							MoveSlider (RightSlider, fixed1, destValue);
						}
					}
				break;			
			case Gdk.Key.Up:
				if (TopSlider.IsEntered) {
					destValue = TopSlider.YGlobal - factorY;
					if (CheckMoving (TopSlider.TYPE, ref destValue, H)) {
						MoveSlider (TopSlider, fixed1, destValue);
					}
				} 
				else if (BottomSlider.IsEntered){
					destValue = BottomSlider.YGlobal - factorY;
					if (CheckMoving (BottomSlider.TYPE, ref destValue, H)) {
						MoveSlider (BottomSlider, fixed1, destValue);
					}					
				}
				break;			
			case Gdk.Key.Down:
				if (TopSlider.IsEntered) {
					destValue = TopSlider.YGlobal + factorY;
					if (CheckMoving (TopSlider.TYPE, ref destValue, H)) {
						MoveSlider (TopSlider, fixed1, destValue);
					}
				} 
				else if (BottomSlider.IsEntered){
					destValue = BottomSlider.YGlobal + factorY;
					if (CheckMoving (BottomSlider.TYPE, ref destValue, H)) {
						MoveSlider (BottomSlider, fixed1, destValue);
					}					
				}
				break;
			}
		}

		#region DrawingAreaImage events

		// todo make static?
		protected void OnDrawingAreaImageExposeEvent (object o, ExposeEventArgs args)
		{
			DrawingArea area = (DrawingArea) o;
			Cairo.Context cr =  Gdk.CairoHelper.Create(area.GdkWindow);

			cr.Save();
			cr.Translate(W / 2.0, H / 2.0);
			// invert angle here, because RotateBilinear filter works counter clockwise
			cr.Rotate (-Angle*Math.PI/180);
			cr.Scale (ScaleForRotation, ScaleForRotation);
			cr.Translate(-W / 2.0, -H / 2.0);
			cr.SetSourceSurface(surface, 0, 0);
			cr.Paint();
			cr.Restore();

			for (int i = 0; i < fixed1.Children.Length; i++) {
				EventBox eb = fixed1.Children[i] as EventBox;
				if (eb == null) {
					continue;
				}
				Slider s = eb.Child as Slider;
				switch (s.TYPE) {
				case Slider.Types.Left:
					cr.Rectangle (0, 0, Math.Round(s.XGlobal), H);
					cr.SetSourceRGBA (RectColor[0], RectColor[1], RectColor[2], RectColor[3]);
					cr.Fill ();					
					break;
				case Slider.Types.Right:
					cr.Rectangle (Math.Round(s.XGlobal), 0, Math.Round(W - s.XGlobal), H);
					cr.SetSourceRGBA (RectColor[0], RectColor[1], RectColor[2], RectColor[3]);
					cr.Fill ();					
					break;

				case Slider.Types.Top:
					cr.Rectangle (0, 0, W, Math.Round(s.YGlobal));
					cr.SetSourceRGBA (RectColor[0], RectColor[1], RectColor[2], RectColor[3]);
					cr.Fill ();					
					break;
				case Slider.Types.Bottom:
					cr.Rectangle (0, Math.Round(s.YGlobal), W, Math.Round(H -  - s.YGlobal));
					cr.SetSourceRGBA (RectColor[0], RectColor[1], RectColor[2], RectColor[3]);
					cr.Fill ();					
					break;
				}
			}				

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
				
			LeftSlider.IsEntered = false;
			RightSlider.IsEntered = false;
			TopSlider.IsEntered = false;
			BottomSlider.IsEntered = false;

			// move slider rectangle 
			if (isPressed) {
				int destX, destY;
				fixed1.GetPointer(out destX, out destY);
				int diffX = xAtPressedBtn - destX;
				int diffY = yAtPressedBtn - destY;

				// check, if a border of image panel is arrived
				float ia = LeftSlider.XGlobal - diffX;
				float ib = RightSlider.XGlobal - diffX;
				float ic = TopSlider.YGlobal - diffY;
				float id = BottomSlider.YGlobal - diffY;

				float tempDiffX = ib - ia;
				float tempDiffY = id - ic;

				if (ia < 0 || ib < 0 || ia > W || ib > W ||
				    ic < 0 || id < 0 || ic > H || id > H ||
					tempDiffX != sliderDiffAfterButtonPressX || tempDiffY != sliderDiffAfterButtonPressY) {
					isPressed = false;
					return;
				}else {
					MoveSlider (LeftSlider, fixed1, ia);
					MoveSlider (RightSlider, fixed1, ib);
					MoveSlider (TopSlider, fixed1, ic);
					MoveSlider (BottomSlider, fixed1, id);

					xAtPressedBtn = xAtPressedBtn - diffX;
					yAtPressedBtn = yAtPressedBtn - diffY;
					fixed1.QueueDraw();
				}									
			}
		}

		protected void OnDrawingAreaImageButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			isPressed = true;
			fixed1.GetPointer(out xAtPressedBtn, out yAtPressedBtn);
			sliderDiffAfterButtonPressX = RightSlider.XGlobal - LeftSlider.XGlobal;
			sliderDiffAfterButtonPressY = BottomSlider.YGlobal - TopSlider.YGlobal;
		}

		protected void OnDrawingAreaImageButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			isPressed = false;
		}

		#endregion DrawingAreaImage events

		#region EventBox events
			
		protected static void OnEventBoxEnterNotify(object sender, EnterNotifyEventArgs a)
		{       
			Slider s = (sender as EventBox).Child as Slider;
			s.IsEntered = true;
			s.Partner.IsEntered = false;
		}
			
		//Mouse click on the controls of the panel  
		protected static void OnEventBoxButtonPressed(object sender, ButtonPressEventArgs a)
		{        
			Slider s = (sender as EventBox).Child as Slider;
			s.IsPressed = true;
		}

		protected static void OnEventBoxButtonReleased(object sender, ButtonReleaseEventArgs a)
		{
			Slider s = (sender as EventBox).Child as Slider;
			s.IsPressed = false;
		}
			
		protected void OnEventBoxMotionNotify (object sender, 
			MotionNotifyEventArgs args)
		{
			Slider s = (sender as EventBox).Child as Slider;
			int intDestX, intDestY;
			fixed1.GetPointer(out intDestX, out intDestY);
			float destX = intDestX, destY = intDestY;

			if (s.IsPressed) {
				// move sliders
				switch (s.TYPE) {
				case Slider.Types.Left:
				case Slider.Types.Right:
					if (CheckMoving (s.TYPE, ref destX, W)) {
						MoveSlider (s, fixed1, destX);
						if (TopSlider.IsEntered && CheckMoving(TopSlider.TYPE, ref destY, H)) {
							MoveSlider (TopSlider, fixed1, destY);
						} else if (BottomSlider.IsEntered && CheckMoving(BottomSlider.TYPE, ref destY, H)) {
							MoveSlider (BottomSlider, fixed1, destY);
						}
					}
					break;
				case Slider.Types.Top:
				case Slider.Types.Bottom:
					if (CheckMoving (s.TYPE, ref destY, H)) {
						MoveSlider (s, fixed1, destY);
						if (LeftSlider.IsEntered && CheckMoving(LeftSlider.TYPE, ref destX, W)) {
							MoveSlider (LeftSlider, fixed1, destX);
						} else if (RightSlider.IsEntered && CheckMoving(RightSlider.TYPE, ref destX, W)) {
							MoveSlider (RightSlider, fixed1, destX);
						}
					}
					break;
				}
			} else {
				// check if second slider is entered
				switch (s.TYPE) {
				case Slider.Types.Left:
				case Slider.Types.Right:
					TopSlider.IsEntered = Math.Abs (TopSlider.YGlobal - destY) < Slider.LINEWIDTH ? true : false;
					BottomSlider.IsEntered = Math.Abs (BottomSlider.YGlobal - destY) < Slider.LINEWIDTH ? true : false;
					break;
				case Slider.Types.Top:
				case Slider.Types.Bottom:
					RightSlider.IsEntered = Math.Abs (RightSlider.XGlobal - destX) < Slider.LINEWIDTH ? true : false;
					LeftSlider.IsEntered = Math.Abs (LeftSlider.XGlobal - destX) < Slider.LINEWIDTH ? true : false;
					break;
				}
			}

			fixed1.QueueDraw();
		}

		#endregion EventBox events

		/// <summary> Removes all sliders and their parental eventboxes as well as the surface of the DrawingAreaImage.</summary>
		private void DisposeFixedWidgetChildrenAndSurface()
		{
			for (int i = 0; i < fixed1.Children.Length; i++)
			{
				Widget w = fixed1.Children [i];
				if (w is EventBox) {					
					fixed1.Remove (w);
					i--;
					w.Destroy ();
					w.Dispose ();
					w = null;
				}
			}

			if (surface != null) {
				surface.Dispose ();
				surface = null;
			}
		}

		/// <summary> 
		/// Checks (and returns true) whether slider can be moved under saving MINDISTANCE.
		/// Also changing destValue when imagePanel border will be arrived.
		/// </summary>
		private bool CheckMoving(Slider.Types t, ref float destValue, int WorH)
		{
			bool moving = true;
			if (destValue < 0) {
				destValue = 0;
				// movingCorrectly = false;
			}
			else if (destValue > WorH) {
				destValue = WorH;
				// movingCorrectly = false;
			}

			switch (t) {
			case Slider.Types.Left:
				if (MINDISTANCE >= RightSlider.XGlobal - destValue) {
					moving = false;
				}
				break;
			case Slider.Types.Right:
				if (MINDISTANCE >= destValue - LeftSlider.XGlobal) {
					moving = false;
				}
				break;	

			case Slider.Types.Top:
				if (MINDISTANCE >= BottomSlider.YGlobal - destValue) {
					moving = false;
				}
				break;
			case Slider.Types.Bottom:
				if (MINDISTANCE >= destValue - TopSlider.YGlobal) {
					moving = false;
				}
				break;
			}

			return moving;
		}

		private static void MoveSlider(Slider s, Fixed fixed1, float destValue)
		{
			int intDestV = (int)Math.Round(destValue);

			switch (s.TYPE) {
			case Slider.Types.Left:
			case Slider.Types.Right:
				fixed1.Move (GetEventBox (s), intDestV, 0);
				s.XGlobal = destValue;
				break;
			case Slider.Types.Top:
			case Slider.Types.Bottom:
				fixed1.Move (GetEventBox (s), 0, intDestV);
				s.YGlobal = destValue;
				break;
			}
		}

		private void AddSliders()
		{
			int x, y;

			// left slider
			x = y = 0;
			LeftSlider = CreateSliderAndEventBox(Slider.Types.Left, fixed1, x, y, Slider.LINEWIDTH, H, OnEventBoxMotionNotify);

			// right slider
			x = W;
			y = 0;
			RightSlider = CreateSliderAndEventBox(Slider.Types.Right, fixed1, x, y, Slider.LINEWIDTH, H, OnEventBoxMotionNotify);

			// top slider
			x = y = 0;
			TopSlider = CreateSliderAndEventBox(Slider.Types.Top, fixed1, x, y, W, Slider.LINEWIDTH, OnEventBoxMotionNotify);

			// bottom slider
			x = 0;
			y = H;
			BottomSlider = CreateSliderAndEventBox(Slider.Types.Bottom, fixed1, x, y, W, Slider.LINEWIDTH, OnEventBoxMotionNotify);

			LeftSlider.Partner = RightSlider;
			RightSlider.Partner = LeftSlider;
			TopSlider.Partner = BottomSlider;
			BottomSlider.Partner = TopSlider;
			this.ShowAll();
		}

		#region Static private functions
		/// <summary>Creates a slider and its parental eventbox.</summary>
		private static Slider CreateSliderAndEventBox(Slider.Types t, Fixed fixed1, int xGlobal, int yGlobal, int w, int h, MotionNotifyEventHandler handler)
		{ 
			Slider s = new Slider(t, xGlobal, yGlobal, w, h);
			EventBox box = new EventBox();
			box.Add(s);
			// event 'all pointer motion'
			box.Events = ((global::Gdk.EventMask)(4));
			box.ButtonPressEvent+=new ButtonPressEventHandler(OnEventBoxButtonPressed);
			box.ButtonReleaseEvent+=new ButtonReleaseEventHandler(OnEventBoxButtonReleased);
			box.EnterNotifyEvent += new EnterNotifyEventHandler (OnEventBoxEnterNotify);
			box.MotionNotifyEvent += new MotionNotifyEventHandler (handler); // (OnEventBoxMotionNotify);
			fixed1.Put(box, xGlobal, yGlobal);

			return s;
		}

		private static EventBox GetEventBox(Slider s)
		{
			return s.Parent as EventBox;
		}

//		private static void CalcNewImageSizeByRotating(int w_old, int h_old, double angle, 
//		                           out Size newSize, out float ue_half, out double scaleForRotation)
//		{
//			newSize = Picturez_Lib.RotateBilinear.CalculateNewImageSize (angle, w_old, h_old, false);
//			double scaleX = (double)w_old / newSize.Width;
//			double scaleY = (double)h_old / newSize.Height;
//			scaleForRotation = Math.Min (scaleX, scaleY);
//
//			float ar1 = (float)w_old/ h_old;
//			float ar2 = (float)newSize.Width/ newSize.Height;
//			float ue = (newSize.Width - w_old) * (float)1;
//			// float ue = newSize.Width * (ar1 - ar2) * (float)scaleForRotation;
//			// float xStart = (float)Math.Round ((imagepanel1.LeftSlider.XGlobal - ue_half) * b1.Width * ar1 / (float)imagepanel1.W);
//			ue_half = ue / 2.0f;
//		}

		#endregion Static private functions
	}
}

