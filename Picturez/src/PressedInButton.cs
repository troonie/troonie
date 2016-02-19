using System;
using Gtk;
using CairoColor = Cairo.Color;
using Cairo;
using Cc = Picturez.ColorConverter;
using System.Diagnostics;
using Picturez_Lib;

namespace Picturez
{	
	[System.ComponentModel.ToolboxItem (true)]
	public class PressedInButton : EventBox
	{
		private Cc.Btn newWorkingColor;
		private bool isButtonReleased, isEntered;
		private DrawingArea da; // = new DrawingArea();
		bool firstClick;
		private Stopwatch sw_doubleClick;

		public string FullText { get; set; }
		public bool IsPressedin { get; private set;	}
		public int BorderlineWidth { get; set; }
		public string Font { get; set; }
		public string Text { get; set; }
		public double TextSize { get; set; }
		// public bool Underline { get; set; }
		public FontWeight Bold { get; set; }
		public bool Italic { get; set; }
		// public Pango.Alignment Alignment { get; set; }

		public PressedInButton ()
		{
			da = new Gtk.DrawingArea();
			firstClick = true;
			sw_doubleClick = new Stopwatch();
			Font = "Arial";
			BorderlineWidth = 2;
			HeightRequest = 25;
			WidthRequest = 20;
			TextSize = 10;
			Text = "Test";
			newWorkingColor = Cc.BtnNormal.I;

			da.ExposeEvent += OnDrawingAreaExposeEvent;
			this.Add(da);

			// this.Events = ((global::Gdk.EventMask)(4)); // motionnotify
			EnterNotifyEvent += OnEnterNotify;
			// this.MotionNotifyEvent += OnMotionNotify;
			LeaveNotifyEvent += OnLeaveNotify;
			ButtonPressEvent+=new ButtonPressEventHandler(OnButtonPressed);
			ButtonReleaseEvent+=new ButtonReleaseEventHandler(OnButtonReleased);
			// Focused += OnFocused;
		}
			
		public void SetPressedIn (bool p_IsPressedin)
		{
			IsPressedin = p_IsPressedin;
			if (IsPressedin) {
				newWorkingColor = Cc.BtnPressedin.I;
			} else {
				newWorkingColor = Cc.BtnNormal.I;
			}

			da.QueueDraw ();			
		}

		protected void OnButtonPressed(object sender, ButtonPressEventArgs a)
		{     
			if (firstClick) {
				sw_doubleClick.Restart ();	
				firstClick = false;
			} else {
				sw_doubleClick.Stop ();
				if (sw_doubleClick.ElapsedMilliseconds < Constants.TIME_DOUBLECLICK) {
					Process p = new Process ();
					p.StartInfo.FileName = Constants.I.EXEPATH + Constants.EXENAME;
					p.StartInfo.Arguments = " -e \"" + FullText + "\"";
					p.Start ();
				}
				firstClick = true;
			}

			IsPressedin = !IsPressedin;
			newWorkingColor = Cc.BtnPressed.I;
			da.QueueDraw ();
		}

		protected void OnButtonReleased(object sender, ButtonReleaseEventArgs a)
		{			
			isButtonReleased = true;
			SetPressedIn (IsPressedin);
		}

		public void OnEnterNotify(object sender, EnterNotifyEventArgs a)
		{       
			isEntered = true;
			da.QueueDraw ();
		}

		protected void OnLeaveNotify(object sender, LeaveNotifyEventArgs a)
		{      
			if (isButtonReleased) {
				isButtonReleased = false;
				return;
			}
				
			isEntered = false;
			da.QueueDraw ();
		}			

		protected void OnDrawingAreaExposeEvent (object obj, ExposeEventArgs args)
		{
			DrawingArea drawingArea = obj as DrawingArea;

			int width = drawingArea.Allocation.Width;
			int height = drawingArea.Allocation.Height;

			Cairo.Context cr =  Gdk.CairoHelper.Create(drawingArea.GdkWindow);

			// CairoColor ac = isSelected ? cc.Cairo_AliceBlue : workingColor;

			LinearGradient lg3 = new LinearGradient(height / 2.0f, 0.0,  height / 2.0f, height);
//			lg3.AddColorStop(0.1, cc.Cairo_White );
//			lg3.AddColorStop(0.5, workingColor );
//			lg3.AddColorStop(0.9, cc.Cairo_White );

			lg3.AddColorStop(0.1, newWorkingColor.Top );
			lg3.AddColorStop(0.5, newWorkingColor.Middle );
			lg3.AddColorStop(0.9, newWorkingColor.Down);

			cr.Rectangle(0, 0, width, height);

			if (isEntered)
				cr.SetSourceRGB(Cc.Instance.Cairo_Orange.R, Cc.Instance.Cairo_Orange.G, Cc.Instance.Cairo_Orange.B);
			else
				cr.SetSourceRGB(newWorkingColor.Border.R, newWorkingColor.Border.G, newWorkingColor.Border.B);
			cr.LineWidth = BorderlineWidth;
			cr.Fill ();

			cr.Rectangle(BorderlineWidth, BorderlineWidth, width - 2 * BorderlineWidth, height - 2 * BorderlineWidth);
			cr.SetSource(lg3);
			cr.Fill ();

			cr.SetSourceRGB(newWorkingColor.Font.R, newWorkingColor.Font.G, newWorkingColor.Font.B);
			cr.SelectFontFace(Font, FontSlant.Normal, FontWeight.Bold);
			cr.SetFontSize(TextSize);

			cr.MoveTo(10, height / 2.0f + 0.1 * height); // 10 % more down for optic
			cr.ShowText(Text);


			lg3.Dispose();        

			((IDisposable) cr.GetTarget()).Dispose ();                                      
			((IDisposable) cr).Dispose ();


		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}

//		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
//		{
//			// Calculate desired size here.
//			requisition.Height = 50;
//			requisition.Width = 50;
//		}
	}
}

