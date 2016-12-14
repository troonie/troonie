using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public partial class ConfirmingDeleteImagesWindow : Gtk.Window
	{
		private Config config;

		private VBox vbox2;

		private Fixed fixed1;

		private Label label1;

		private Label label2;

		private HSeparator hseparator1;

		private CheckButton chkBtn;

		private HSeparator hseparator2;

		private Fixed fixed2;

		private HBox hbox1;

		private TroonieButton picbtnYes;

		private TroonieButton picbtnNo;

		public OnReleasedButtonDelegate OnReleasedOkButton;
//		public OnReleasedButtonDelegate OnReleasedCancelButton;


		public ConfirmingDeleteImagesWindow (Config config) : 
		base(Gtk.WindowType.Toplevel)
		{
			this.config = config;
			this.KeepAbove = true;
			Build ();
			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);

			Title = Language.I.L[29];
			label1.LabelProp = "<b>" + Language.I.L[192] + "</b>";
			label2.Text = Language.I.L[171];
			chkBtn.Label = Language.I.L[193];

			picbtnYes.Text = Language.I.L [61];
			picbtnNo.Text = Language.I.L [62];
		}

		protected virtual void Build ()
		{
//			global::Stetic.Gui.Initialize (this);
			// Widget Troonie.AskForDesktopContextMenuWindow
//			Name = "Troonie.ConfirmingDeleteImagesWindow";
//			Title = global::Mono.Unix.Catalog.GetString ("ConfirmingDeleteImagesWindow");
			WindowPosition = ((WindowPosition)(4));
			// Container child Troonie.AskForDesktopContextMenuWindow.Gtk.Container+ContainerChild
			vbox2 = new VBox ();
			vbox2.Name = "vbox2";
			vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			fixed1 = new Fixed ();
			fixed1.HeightRequest = 10;
			fixed1.Name = "fixed1";
			fixed1.HasWindow = false;
			vbox2.Add (fixed1);
			Box.BoxChild w1 = ((Box.BoxChild)(vbox2 [fixed1]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			label1 = new Label ();
			label1.Name = "label1";
			label1.LabelProp = global::Mono.Unix.Catalog.GetString ("label1");
			label1.UseMarkup = true;
			vbox2.Add (label1);
			Box.BoxChild w2 = ((Box.BoxChild)(vbox2 [label1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			label2 = new Label ();
			label2.Name = "label2";
			label2.LabelProp = global::Mono.Unix.Catalog.GetString ("label2");
			vbox2.Add (label2);
			Box.BoxChild w3 = ((Box.BoxChild)(vbox2 [label2]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			hseparator1 = new HSeparator ();
			hseparator1.Name = "hseparator1";
			vbox2.Add (hseparator1);
			Box.BoxChild w4 = ((Box.BoxChild)(vbox2 [hseparator1]));
			w4.Position = 3;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			chkBtn = new CheckButton ();
			chkBtn.CanFocus = true;
			chkBtn.Name = "chkBtn";
			chkBtn.Label = global::Mono.Unix.Catalog.GetString ("checkbutton1");
			chkBtn.Active = true;
			chkBtn.DrawIndicator = true;
			chkBtn.UseUnderline = true;
			vbox2.Add (chkBtn);
			Box.BoxChild w5 = ((Box.BoxChild)(vbox2 [chkBtn]));
			w5.Position = 4;
			w5.Expand = false;
			w5.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			hseparator2 = new HSeparator ();
			hseparator2.Name = "hseparator2";
			vbox2.Add (hseparator2);
			Box.BoxChild w6 = ((Box.BoxChild)(vbox2 [hseparator2]));
			w6.Position = 5;
			w6.Expand = false;
			w6.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			fixed2 = new Fixed ();
			fixed2.HeightRequest = 10;
			fixed2.Name = "fixed2";
			fixed2.HasWindow = false;
			vbox2.Add (fixed2);
			Box.BoxChild w7 = ((Box.BoxChild)(vbox2 [fixed2]));
			w7.PackType = ((PackType)(1));
			w7.Position = 6;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			hbox1 = new HBox ();
			hbox1.Name = "hbox1";
			hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			picbtnYes = new global::Troonie.TroonieButton ();
			picbtnYes.Name = "picbtnYes";
			picbtnYes.CheckReleaseState = true;
			picbtnYes.BorderlineWidth = 2;
			picbtnYes.ButtonHeight = 30;
			picbtnYes.ButtonWidth = 0;
			picbtnYes.Font = "Arial";
			picbtnYes.Text = "Yes";
			picbtnYes.TextSize = 10;
			hbox1.Add (picbtnYes);
			Box.BoxChild w8 = ((Box.BoxChild)(hbox1 [picbtnYes]));
			w8.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			picbtnNo = new global::Troonie.TroonieButton ();
			picbtnNo.Name = "picbtnNo";
			picbtnNo.CheckReleaseState = false;
			picbtnNo.BorderlineWidth = 2;
			picbtnNo.ButtonHeight = 30;
			picbtnNo.ButtonWidth = 0;
			picbtnNo.Font = "Arial";
			picbtnNo.Text = "No";
			picbtnNo.TextSize = 10;
			hbox1.Add (picbtnNo);
			Box.BoxChild w9 = ((Box.BoxChild)(hbox1 [picbtnNo]));
			w9.Position = 1;
			vbox2.Add (hbox1);
			Box.BoxChild w10 = ((Box.BoxChild)(vbox2 [hbox1]));
			w10.PackType = ((PackType)(1));
			w10.Position = 7;
			w10.Expand = false;
			w10.Fill = false;
			Add (vbox2);
			if ((Child != null)) {
				Child.ShowAll ();
			}
			DefaultWidth = 400;
			DefaultHeight = 161;
			Show ();
			chkBtn.Toggled += new global::System.EventHandler (OnChkBtnToggled);
			picbtnYes.ButtonReleaseEvent += new ButtonReleaseEventHandler (OnPicbtnYesButtonReleaseEvent);
			picbtnNo.ButtonReleaseEvent += new ButtonReleaseEventHandler (OnPicbtnNoButtonReleaseEvent);
			KeyPressEvent += new KeyPressEventHandler (OnKeyPressEvent);
		}

		protected void OnPicbtnYesButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//fire the event now
			if (OnReleasedOkButton != null) //is there a EventHandler?
			{
				Config.Save (Constants.I.CONFIG);

				OnReleasedOkButton.Invoke(); //calls its EventHandler                
			} 
				
			this.DestroyAll ();
		}	

		protected void OnPicbtnNoButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			this.Destroy ();
		}

		protected void OnChkBtnToggled (object sender, EventArgs e)
		{
			config.ConfirmDeleteImages = chkBtn.Active;
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			//			System.Console.WriteLine("Keypress: {0}  -->  State: {1}", args.Event.Key, args.Event.State); 

			if (args.Event.Key == Gdk.Key.Return) {
				OnPicbtnYesButtonReleaseEvent(o, null);
			}
		}
	}
}

