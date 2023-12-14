using System;
using System.IO;
using Gtk;
using Pango;
using Troonie_Lib;

namespace Troonie
{
	/// <summary>Event handler for changing slider values (XGlobal or YGlobal).</summary>
	public delegate void OnHyperTextLabelTextChangedEventHandler();

	[System.ComponentModel.ToolboxItem (true)]
	public class HyperTextLabel : Gtk.EventBox
	{		
		private DrawingArea da = new DrawingArea();
		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;

		public FileChooserAction FileChooserAction { get; set; }

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

			da.Drawn += OnDrawingAreaExposeEvent;
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

			FileChooserAction = FileChooserAction.SelectFolder; // default
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
					FileChooserAction,
					o);

			if (filechooser.Run() == (int)ResponseType.Ok) 
			{
				Text = filechooser.Filename;

				// avoid folders with white spaces at the end
				if (FileChooserAction == FileChooserAction.SelectFolder) {
					Text = Text.TrimEnd();
					if (!Directory.Exists(Text)) {
						Directory.CreateDirectory(Text);
					}
				}

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
			

		protected void OnDrawingAreaExposeEvent (object obj, DrawnArgs args)
		{
			DrawingArea drawingArea = obj as DrawingArea;

			int width = drawingArea.Allocation.Width;

			Renderer renderer = new Renderer(drawingArea.Screen.Handle);
			//renderer.Drawable = drawingArea.GdkWindow;
			//renderer.Gc = drawingArea.Style.BlackGC;

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

			renderer.DrawLayout(layout, 0, 0);
			renderer.SetColor(RenderPart.Foreground, new Pango.Color () { Red = TextColor.Red, Blue = TextColor.Blue, Green = TextColor.Green });
			renderer.SetColor(RenderPart.Underline, new Pango.Color() { Red = TextColor.Red, Blue = TextColor.Blue, Green = TextColor.Green });

            layout.Alignment = Alignment;
			renderer.DrawLayout(layout, 0, 0);

			renderer.SetColor(RenderPart.Foreground, Pango.Color.Zero);

//			((IDisposable) renderer.Drawable).Dispose();      
//			((IDisposable) renderer.Gc).Dispose();
//			((IDisposable) renderer).Dispose();
			//renderer.Drawable = null;
			//renderer.Gc = null;


		}			
	}
}

