
// This file has been generated by the GUI designer. Do not modify.
namespace Troonie
{
	public partial class OptionsWidget
	{
		private global::Gtk.ScrolledWindow scrolledwindow1;
		
		private global::Gtk.EventBox eventbox1;
		
		private global::Gtk.Notebook notebook1;
		
		private global::Gtk.EventBox eventboxPage1;
		
		private global::Gtk.VBox vboxPage1;
		
		private global::Gtk.Frame frameMaxSideLengthFilterImage;
		
		private global::Gtk.Alignment AlignmentMaxSideLengthFilterImage;
		
		private global::Gtk.HBox hboxMaxSideLengthFilterImage;
		
		private global::Troonie.TroonieButton btnPlusMaxSideLengthFilterImage;
		
		private global::Gtk.Entry entryMaxSideLengthFilterImage;
		
		private global::Troonie.TroonieButton btnMinusMaxSideLengthFilterImage;
		
		private global::Gtk.Label lbMaxSideLengthFilterImage;
		
		private global::Gtk.Frame frameVideoplayer;
		
		private global::Gtk.Alignment AlignmentVideoplayer;
		
		private global::Gtk.HBox hboxVideoplayer;
		
		private global::Troonie.HyperTextLabel hypertextlabelVideoplayer;
		
		private global::Gtk.Label lbFrameVideoplayer;
		
		private global::Gtk.Label lbPage1;
		
		private global::Gtk.EventBox eventboxPage2;
		
		private global::Gtk.Table table1;
		
		private global::Gtk.Label lbPage2;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Troonie.OptionsWidget
			this.Name = "Troonie.OptionsWidget";
			this.Title = global::Mono.Unix.Catalog.GetString ("OptionsWidget");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Troonie.OptionsWidget.Gtk.Container+ContainerChild
			this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
			this.scrolledwindow1.CanFocus = true;
			this.scrolledwindow1.Name = "scrolledwindow1";
			this.scrolledwindow1.ShadowType = ((global::Gtk.ShadowType)(3));
			// Container child scrolledwindow1.Gtk.Container+ContainerChild
			global::Gtk.Viewport w1 = new global::Gtk.Viewport ();
			w1.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child GtkViewport.Gtk.Container+ContainerChild
			this.eventbox1 = new global::Gtk.EventBox ();
			this.eventbox1.Name = "eventbox1";
			// Container child eventbox1.Gtk.Container+ContainerChild
			this.notebook1 = new global::Gtk.Notebook ();
			this.notebook1.CanFocus = true;
			this.notebook1.Name = "notebook1";
			this.notebook1.CurrentPage = 1;
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.eventboxPage1 = new global::Gtk.EventBox ();
			this.eventboxPage1.Name = "eventboxPage1";
			// Container child eventboxPage1.Gtk.Container+ContainerChild
			this.vboxPage1 = new global::Gtk.VBox ();
			this.vboxPage1.Name = "vboxPage1";
			this.vboxPage1.Spacing = 6;
			// Container child vboxPage1.Gtk.Box+BoxChild
			this.frameMaxSideLengthFilterImage = new global::Gtk.Frame ();
			this.frameMaxSideLengthFilterImage.Name = "frameMaxSideLengthFilterImage";
			this.frameMaxSideLengthFilterImage.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frameMaxSideLengthFilterImage.Gtk.Container+ContainerChild
			this.AlignmentMaxSideLengthFilterImage = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentMaxSideLengthFilterImage.Name = "AlignmentMaxSideLengthFilterImage";
			this.AlignmentMaxSideLengthFilterImage.LeftPadding = ((uint)(12));
			// Container child AlignmentMaxSideLengthFilterImage.Gtk.Container+ContainerChild
			this.hboxMaxSideLengthFilterImage = new global::Gtk.HBox ();
			this.hboxMaxSideLengthFilterImage.Name = "hboxMaxSideLengthFilterImage";
			this.hboxMaxSideLengthFilterImage.Spacing = 6;
			// Container child hboxMaxSideLengthFilterImage.Gtk.Box+BoxChild
			this.btnPlusMaxSideLengthFilterImage = new global::Troonie.TroonieButton ();
			this.btnPlusMaxSideLengthFilterImage.Name = "btnPlusMaxSideLengthFilterImage";
			this.btnPlusMaxSideLengthFilterImage.CheckReleaseState = true;
			this.btnPlusMaxSideLengthFilterImage.BorderlineWidth = 2;
			this.btnPlusMaxSideLengthFilterImage.ButtonHeight = 35;
			this.btnPlusMaxSideLengthFilterImage.ButtonWidth = 0;
			this.btnPlusMaxSideLengthFilterImage.Font = "Arial";
			this.btnPlusMaxSideLengthFilterImage.Text = "+";
			this.btnPlusMaxSideLengthFilterImage.TextSize = 12D;
			this.hboxMaxSideLengthFilterImage.Add (this.btnPlusMaxSideLengthFilterImage);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hboxMaxSideLengthFilterImage [this.btnPlusMaxSideLengthFilterImage]));
			w2.Position = 0;
			// Container child hboxMaxSideLengthFilterImage.Gtk.Box+BoxChild
			this.entryMaxSideLengthFilterImage = new global::Gtk.Entry ();
			this.entryMaxSideLengthFilterImage.CanFocus = true;
			this.entryMaxSideLengthFilterImage.Name = "entryMaxSideLengthFilterImage";
			this.entryMaxSideLengthFilterImage.IsEditable = true;
			this.entryMaxSideLengthFilterImage.InvisibleChar = '●';
			this.hboxMaxSideLengthFilterImage.Add (this.entryMaxSideLengthFilterImage);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hboxMaxSideLengthFilterImage [this.entryMaxSideLengthFilterImage]));
			w3.Position = 1;
			// Container child hboxMaxSideLengthFilterImage.Gtk.Box+BoxChild
			this.btnMinusMaxSideLengthFilterImage = new global::Troonie.TroonieButton ();
			this.btnMinusMaxSideLengthFilterImage.Name = "btnMinusMaxSideLengthFilterImage";
			this.btnMinusMaxSideLengthFilterImage.CheckReleaseState = true;
			this.btnMinusMaxSideLengthFilterImage.BorderlineWidth = 2;
			this.btnMinusMaxSideLengthFilterImage.ButtonHeight = 35;
			this.btnMinusMaxSideLengthFilterImage.ButtonWidth = 0;
			this.btnMinusMaxSideLengthFilterImage.Font = "Arial";
			this.btnMinusMaxSideLengthFilterImage.Text = "-";
			this.btnMinusMaxSideLengthFilterImage.TextSize = 12D;
			this.hboxMaxSideLengthFilterImage.Add (this.btnMinusMaxSideLengthFilterImage);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hboxMaxSideLengthFilterImage [this.btnMinusMaxSideLengthFilterImage]));
			w4.Position = 2;
			this.AlignmentMaxSideLengthFilterImage.Add (this.hboxMaxSideLengthFilterImage);
			this.frameMaxSideLengthFilterImage.Add (this.AlignmentMaxSideLengthFilterImage);
			this.lbMaxSideLengthFilterImage = new global::Gtk.Label ();
			this.lbMaxSideLengthFilterImage.Name = "lbMaxSideLengthFilterImage";
			this.lbMaxSideLengthFilterImage.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>lbMaxSideLengthFilterImage</b>");
			this.lbMaxSideLengthFilterImage.UseMarkup = true;
			this.frameMaxSideLengthFilterImage.LabelWidget = this.lbMaxSideLengthFilterImage;
			this.vboxPage1.Add (this.frameMaxSideLengthFilterImage);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxPage1 [this.frameMaxSideLengthFilterImage]));
			w7.Position = 1;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vboxPage1.Gtk.Box+BoxChild
			this.frameVideoplayer = new global::Gtk.Frame ();
			this.frameVideoplayer.Name = "frameVideoplayer";
			this.frameVideoplayer.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frameVideoplayer.Gtk.Container+ContainerChild
			this.AlignmentVideoplayer = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentVideoplayer.Name = "AlignmentVideoplayer";
			this.AlignmentVideoplayer.LeftPadding = ((uint)(12));
			// Container child AlignmentVideoplayer.Gtk.Container+ContainerChild
			this.hboxVideoplayer = new global::Gtk.HBox ();
			this.hboxVideoplayer.Name = "hboxVideoplayer";
			this.hboxVideoplayer.Spacing = 6;
			// Container child hboxVideoplayer.Gtk.Box+BoxChild
			this.hypertextlabelVideoplayer = new global::Troonie.HyperTextLabel ();
			this.hypertextlabelVideoplayer.Sensitive = false;
			this.hypertextlabelVideoplayer.Name = "hypertextlabelVideoplayer";
			this.hypertextlabelVideoplayer.Sensitive = false;
			this.hypertextlabelVideoplayer.Text = "Starttext";
			this.hypertextlabelVideoplayer.TextSize = 0;
			this.hypertextlabelVideoplayer.ShownTextLength = 0;
			this.hypertextlabelVideoplayer.Underline = false;
			this.hypertextlabelVideoplayer.Bold = false;
			this.hypertextlabelVideoplayer.Italic = false;
			this.hboxVideoplayer.Add (this.hypertextlabelVideoplayer);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hboxVideoplayer [this.hypertextlabelVideoplayer]));
			w8.Position = 0;
			this.AlignmentVideoplayer.Add (this.hboxVideoplayer);
			this.frameVideoplayer.Add (this.AlignmentVideoplayer);
			this.lbFrameVideoplayer = new global::Gtk.Label ();
			this.lbFrameVideoplayer.Name = "lbFrameVideoplayer";
			this.lbFrameVideoplayer.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>lbFrameVideoplayer</b>");
			this.lbFrameVideoplayer.UseMarkup = true;
			this.frameVideoplayer.LabelWidget = this.lbFrameVideoplayer;
			this.vboxPage1.Add (this.frameVideoplayer);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vboxPage1 [this.frameVideoplayer]));
			w11.Position = 2;
			w11.Expand = false;
			w11.Fill = false;
			this.eventboxPage1.Add (this.vboxPage1);
			this.notebook1.Add (this.eventboxPage1);
			// Notebook tab
			this.lbPage1 = new global::Gtk.Label ();
			this.lbPage1.Name = "lbPage1";
			this.lbPage1.LabelProp = global::Mono.Unix.Catalog.GetString ("lbPage1");
			this.notebook1.SetTabLabel (this.eventboxPage1, this.lbPage1);
			this.lbPage1.ShowAll ();
			// Container child notebook1.Gtk.Notebook+NotebookChild
			this.eventboxPage2 = new global::Gtk.EventBox ();
			this.eventboxPage2.Name = "eventboxPage2";
			// Container child eventboxPage2.Gtk.Container+ContainerChild
			this.table1 = new global::Gtk.Table (((uint)(3)), ((uint)(3)), false);
			this.table1.Name = "table1";
			this.table1.RowSpacing = ((uint)(6));
			this.table1.ColumnSpacing = ((uint)(6));
			this.eventboxPage2.Add (this.table1);
			this.notebook1.Add (this.eventboxPage2);
			global::Gtk.Notebook.NotebookChild w15 = ((global::Gtk.Notebook.NotebookChild)(this.notebook1 [this.eventboxPage2]));
			w15.Position = 1;
			// Notebook tab
			this.lbPage2 = new global::Gtk.Label ();
			this.lbPage2.Name = "lbPage2";
			this.lbPage2.LabelProp = global::Mono.Unix.Catalog.GetString ("lbPage2");
			this.notebook1.SetTabLabel (this.eventboxPage2, this.lbPage2);
			this.lbPage2.ShowAll ();
			this.eventbox1.Add (this.notebook1);
			w1.Add (this.eventbox1);
			this.scrolledwindow1.Add (w1);
			this.Add (this.scrolledwindow1);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 300;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.btnPlusMaxSideLengthFilterImage.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnButtonReleaseEvent);
			this.btnMinusMaxSideLengthFilterImage.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnButtonReleaseEvent);
		}
	}
}
