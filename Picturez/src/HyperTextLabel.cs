using System;
using Gtk;
using Pango;
using Picturez_Lib;

namespace Picturez
{
	/// <summary>Event handler for changing slider values (XGlobal or YGlobal).</summary>
	public delegate void OnHyperTextLabelTextChangedEventHandler();

	[System.ComponentModel.ToolboxItem (true)]
	public class HyperTextLabel : Gtk.EventBox
	{		
		private DrawingArea da = new DrawingArea();
		private Picturez.ColorConverter colorConverter = Picturez.ColorConverter.Instance;

		public new bool Sensitive {			
			get {
				return base.Sensitive; 
			}
			set {
				base.Sensitive = value;
				if (value) {
					TextColor = colorConverter.Blue;
				} else {
					TextColor = colorConverter.White;
				}
				QueueDraw ();
			} 
		}
		public string Font { get; set; }
		string text;
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
				FireHyperTextLabelTextChangedEvent ();
				// Force redrawing
				QueueDraw ();
			}
		}
		public Gdk.Color TextColor { get; set; }
		public int TextSize { get; set; }
		public int ShownTextLength { get; set; }
		public bool Underline { get; set; }
		public bool Bold { get; set; }
		public bool Italic { get; set; }
		public Pango.Alignment Alignment { get; set; }
		/// <summary>Handles the event at the client.</summary>
		public OnHyperTextLabelTextChangedEventHandler OnHyperTextLabelTextChanged;

		public HyperTextLabel ()
		{
			da = new Gtk.DrawingArea();
			da.ModifyBg(StateType.Normal, colorConverter.White);

			da.ExposeEvent += OnDrawingAreaExposeEvent;
			// Does not work here, when HyperTextLabel will be used by GUI Designer
			InitDefaultValues ();

			this.Add(da);
		}

		public override void Destroy ()
		{
			da.Destroy ();
			da.Dispose ();
			base.Destroy ();
//			base.Dispose ();
		}

		public void InitDefaultValues()
		{
			// default values
			Sensitive = true;
			ShownTextLength = 50;
			// TextColor = colorConverter.Blue;
			Alignment = Pango.Alignment.Left;
			TextSize = 9;
			Bold = true;
			Italic = false;
			text = "Demo text";
			HeightRequest = 15;
			Font = "Serif";
		}

		/// <summary>Fires the slider changed value event.</summary>
		private void FireHyperTextLabelTextChangedEvent()
		{
			//fire the event now
			if (this.OnHyperTextLabelTextChanged != null) //is there a EventHandler?
			{
				this.OnHyperTextLabelTextChanged.Invoke(); //calls its EventHandler                
			}
			else { } //if not, ignore
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			object[] o = new object[]{"Cancel",ResponseType.Cancel,
				"OK",ResponseType.Ok};

			Gtk.FileChooserDialog filechooser =
				new Gtk.FileChooserDialog("Choose the file to save",
					null,
					FileChooserAction.SelectFolder,
					o);

			if (filechooser.Run() == (int)ResponseType.Ok) 
			{
				Text = filechooser.Filename;
//				// force redraw
//				QueueDraw();
			}

			filechooser.Destroy();
			return base.OnButtonPressEvent (ev);
		}

		protected override bool OnVisibilityNotifyEvent (Gdk.EventVisibility evnt)
		{
			return base.OnVisibilityNotifyEvent (evnt);
		}
			

		protected void OnDrawingAreaExposeEvent (object obj, ExposeEventArgs args)
		{
			DrawingArea drawingArea = obj as DrawingArea;

			int width = drawingArea.Allocation.Width;

			Gdk.PangoRenderer renderer = Gdk.PangoRenderer.GetDefault(drawingArea.Screen);
			renderer.Drawable = drawingArea.GdkWindow;
			renderer.Gc = drawingArea.Style.BlackGC;

			Context context = drawingArea.CreatePangoContext();
			Pango.Layout layout = new Pango.Layout(context);

			layout.Width = Pango.Units.FromPixels(width);

			string showText = text;
			if (text.Length > ShownTextLength)
			{
				int start = text.Length - ShownTextLength + 3;
				showText = "..." + text.Substring(start);
			}

			string markupText = Underline ? "<u>" + showText + "</u>" : showText; 

			layout.SetMarkup (markupText);
			//layout.SetText("Australia");

			string useBold = Bold ? " Bold " : "";
			string useItalic = Italic ? " Italic " : "";
			string fontSizeString = " " + TextSize.ToString ();
			string fontDescAsString = Font + useBold + useItalic + fontSizeString;

			// FontDescription desc = FontDescription.FromString("Serif Bold Italic 10");
			FontDescription desc = FontDescription.FromString(fontDescAsString);
			layout.FontDescription = desc;

			renderer.SetOverrideColor(RenderPart.Foreground, TextColor);
			renderer.SetOverrideColor(RenderPart.Underline, TextColor);

			layout.Alignment = Alignment;
			renderer.DrawLayout(layout, 0, 0);

			renderer.SetOverrideColor(RenderPart.Foreground, Gdk.Color.Zero);

//			((IDisposable) renderer.Drawable).Dispose();      
//			((IDisposable) renderer.Gc).Dispose();
//			((IDisposable) renderer).Dispose();
			renderer.Drawable = null;
			renderer.Gc = null;


		}			
	}
}

