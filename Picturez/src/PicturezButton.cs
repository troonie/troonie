using System;
using Gtk;
using CairoColor = Cairo.Color;
using Cairo;
using Cc = Picturez.ColorConverter;

namespace Picturez
{	
	[System.ComponentModel.ToolboxItem (true)]
	public class PicturezButton : EventBox
	{
		private Cc.Btn workingColor;
		private bool isPressed, isReleased, isEntered;
		private DrawingArea da = new DrawingArea();

		/// <summary>
		/// Set to true, when after every OnButtonReleased() a OnLeaveNotify() directly follows. Default: false.   
		/// </summary>
		public bool CheckReleaseState { get; set; }
		public int BorderlineWidth { get; set; }
		public int ButtonHeight {
			get {
				return this.HeightRequest;
			}
			set {
				this.HeightRequest = value;
			}
		}
		public int ButtonWidth {
			get {
				return this.WidthRequest;
			}
			set {
				this.WidthRequest = value;
			}
		}


//  { get; set; }
		public string Font { get; set; }
		public string Text { get; set; }
		public double TextSize { get; set; }
		/// <summary>
		/// Determines the weight of the font (<code>normal</code>, <code>bold</code>). 
		/// </summary>
		public FontWeight FontWeight { get; set; }
		/// <summary>
		/// Determines the slant of the font (<code>normal</code>, <code>italic</code>, <code>oblique</code>). 
		/// </summary>
		public FontSlant FontSlant { get; set; }

		public PicturezButton ()
		{
			da = new Gtk.DrawingArea();

			#region default values, will be overwritten by using designer
			Font = "Arial";
			BorderlineWidth = 2;
			FontWeight = FontWeight.Bold;
			FontSlant = FontSlant.Normal;
			ButtonHeight = 45;
			ButtonWidth = 45;
			TextSize = 10;
			Text = "Test";
			#endregion default values, will be overwritten by using designer

			workingColor = Cc.BtnNormal.I;

			da.ExposeEvent += OnDrawingAreaExposeEvent;
			this.Add(da);

			EnterNotifyEvent += OnEnterNotify;
			LeaveNotifyEvent += OnLeaveNotify;
			ButtonPressEvent+=new ButtonPressEventHandler(OnButtonPressed);
			ButtonReleaseEvent+=new ButtonReleaseEventHandler(OnButtonReleased);
		}	

//		public override void Destroy ()
//		{
//			da.Destroy ();
//			da.Dispose ();
//			base.Destroy ();
////			base.Dispose ();
//		}

		public void Redraw()
		{
			da.QueueDraw ();
		}

		protected void OnButtonPressed(object sender, ButtonPressEventArgs a)
		{        			
			isPressed = true;
			da.QueueDraw ();
		}

		protected void OnButtonReleased(object sender, ButtonReleaseEventArgs a)
		{			
			isPressed = false;
			isReleased = true;
//			Console.WriteLine ("OnButtonReleased Orig");
			da.QueueDraw ();
		}

		public void OnEnterNotify(object sender, EnterNotifyEventArgs a)
		{       
			isEntered = true;
//			Console.WriteLine ("OnEnterNotify");
			da.QueueDraw ();
		}

		// After every OnButtonReleased() a OnLeaveNotify() follows directly 
		protected void OnLeaveNotify(object sender, LeaveNotifyEventArgs a)
		{  
			if (CheckReleaseState) {
				if (isReleased) {
					isReleased = false;
//					Console.WriteLine ("OnLeaveNotify IF");

				} else {
					isEntered = false;
//					Console.WriteLine ("OnLeaveNotify ELSE");
				}				
			} else {
				isEntered = false;
			}
								
			da.QueueDraw ();
		}					

		protected void OnDrawingAreaExposeEvent (object obj, ExposeEventArgs args)
		{
			DrawingArea drawingArea = obj as DrawingArea;

			int width = drawingArea.Allocation.Width;
			int height = drawingArea.Allocation.Height;

			Cairo.Context cr =  Gdk.CairoHelper.Create(drawingArea.GdkWindow);

			LinearGradient lg3 = new LinearGradient(height / 2.0f, 0.0,  height / 2.0f, height);

			if (isPressed) {
				workingColor = Cc.BtnPressed.I;
			} else if (isEntered) {
				workingColor = Cc.BtnPressedin.I;
			} else {
				workingColor = Cc.BtnNormal.I;
			}

			lg3.AddColorStop(0.1, workingColor.Top );
			lg3.AddColorStop(0.5, workingColor.Middle );
			lg3.AddColorStop(0.9, workingColor.Down);

			cr.Rectangle(0, 0, width, height);

			cr.SetSourceRGB(workingColor.Border.R, workingColor.Border.G, workingColor.Border.B);
			cr.LineWidth = BorderlineWidth;
			cr.Fill ();

			cr.Rectangle(BorderlineWidth, BorderlineWidth, width - 2 * BorderlineWidth, height - 2 * BorderlineWidth);
			cr.SetSource(lg3);
			cr.Fill ();

			cr.SetSourceRGB(workingColor.Font.R, workingColor.Font.G, workingColor.Font.B);
			cr.SelectFontFace(Font, FontSlant, FontWeight);
			cr.SetFontSize(TextSize);

			TextExtents extents = cr.TextExtents(Text);
			// center text
			cr.MoveTo(width / 2.0f - extents.Width / 2, height / 2.0f + extents.Height / 2);
			cr.ShowText(Text);

//			Console.WriteLine ("Painting");

			lg3.Dispose();        

			((IDisposable) cr.GetTarget()).Dispose ();                                      
			((IDisposable) cr).Dispose ();


		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}
	}
}

