
// This file has been generated by the GUI designer. Do not modify.
namespace Troonie
{
	public partial class AskForDesktopContextMenuWindow
	{
		private global::Gtk.VBox vbox2;
		
		private global::Gtk.Fixed fixed1;
		
		private global::Gtk.Label label1;
		
		private global::Gtk.Label label2;
		
		private global::Gtk.HSeparator hseparator1;
		
		private global::Gtk.CheckButton chkBtn;
		
		private global::Gtk.HSeparator hseparator2;
		
		private global::Gtk.Fixed fixed2;
		
		private global::Gtk.HBox hbox1;
		
		private global::Troonie.TroonieButton picbtnYes;
		
		private global::Troonie.TroonieButton picbtnNo;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Troonie.AskForDesktopContextMenuWindow
			this.Name = "Troonie.AskForDesktopContextMenuWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("AskForDesktopContextMenuWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Troonie.AskForDesktopContextMenuWindow.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.fixed1 = new global::Gtk.Fixed ();
			this.fixed1.HeightRequest = 10;
			this.fixed1.Name = "fixed1";
			this.fixed1.HasWindow = false;
			this.vbox2.Add (this.fixed1);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.fixed1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label1 = new global::Gtk.Label ();
			this.label1.Name = "label1";
			this.label1.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			this.label1.UseMarkup = true;
			this.vbox2.Add (this.label1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.label1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.label2 = new global::Gtk.Label ();
			this.label2.Name = "label2";
			this.label2.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			this.vbox2.Add (this.label2);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.label2]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hseparator1 = new global::Gtk.HSeparator ();
			this.hseparator1.Name = "hseparator1";
			this.vbox2.Add (this.hseparator1);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hseparator1]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.chkBtn = new global::Gtk.CheckButton ();
			this.chkBtn.CanFocus = true;
			this.chkBtn.Name = "chkBtn";
			this.chkBtn.Label = global::Mono.Unix.Catalog.GetString ("checkbutton1");
			this.chkBtn.Active = true;
			this.chkBtn.DrawIndicator = true;
			this.chkBtn.UseUnderline = true;
			this.vbox2.Add (this.chkBtn);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.chkBtn]));
			w5.Position = 4;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hseparator2 = new global::Gtk.HSeparator ();
			this.hseparator2.Name = "hseparator2";
			this.vbox2.Add (this.hseparator2);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hseparator2]));
			w6.Position = 5;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.fixed2 = new global::Gtk.Fixed ();
			this.fixed2.HeightRequest = 10;
			this.fixed2.Name = "fixed2";
			this.fixed2.HasWindow = false;
			this.vbox2.Add (this.fixed2);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.fixed2]));
			w7.PackType = ((global::Gtk.PackType)(1));
			w7.Position = 6;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.picbtnYes = new global::Troonie.TroonieButton ();
			this.picbtnYes.Name = "picbtnYes";
			this.picbtnYes.CheckReleaseState = true;
			this.picbtnYes.BorderlineWidth = 2;
			this.picbtnYes.ButtonHeight = 30;
			this.picbtnYes.ButtonWidth = 0;
			this.picbtnYes.Font = "Arial";
			this.picbtnYes.Text = "Yes";
			this.picbtnYes.TextSize = 10D;
			this.hbox1.Add (this.picbtnYes);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.picbtnYes]));
			w8.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.picbtnNo = new global::Troonie.TroonieButton ();
			this.picbtnNo.Name = "picbtnNo";
			this.picbtnNo.CheckReleaseState = false;
			this.picbtnNo.BorderlineWidth = 2;
			this.picbtnNo.ButtonHeight = 30;
			this.picbtnNo.ButtonWidth = 0;
			this.picbtnNo.Font = "Arial";
			this.picbtnNo.Text = "No";
			this.picbtnNo.TextSize = 10D;
			this.hbox1.Add (this.picbtnNo);
			global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.picbtnNo]));
			w9.Position = 1;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w10.PackType = ((global::Gtk.PackType)(1));
			w10.Position = 7;
			w10.Expand = false;
			w10.Fill = false;
			this.Add (this.vbox2);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 161;
			this.Show ();
			this.chkBtn.Toggled += new global::System.EventHandler (this.OnChkBtnToggled);
			this.picbtnYes.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnPicbtnYesButtonReleaseEvent);
			this.picbtnNo.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnPicbtnNoButtonReleaseEvent);
		}
	}
}
