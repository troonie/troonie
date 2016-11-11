using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;

namespace Troonie
{
	public delegate void EnterMetaDataDelegate(string[] elements);

	public partial class EnterMetaDataWindow : Gtk.Window
	{
		private bool addMode;
		private VBox vbox;
		private Entry entry;
		private TroonieButton btnOk;
//		private ViewerWidget vw;
		private Tags tags;
		private List<ViewerImagePanel> pressedInVIPs;

//		public EnterMetaDataDelegate OnReleasedOkButton;

		public String OkButtontext
		{
			get { return btnOk.Text; }
			set { btnOk.Text = value; }
		}

		public EnterMetaDataWindow (List<ViewerImagePanel> pressedInVIPs, Tags tags) : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
//			Parent = vw;
//			this.vw = vw;
			this.tags = tags;
			this.pressedInVIPs = pressedInVIPs;

			WindowPosition = Gtk.WindowPosition.CenterOnParent; // ((WindowPosition)(4));
			// Container child Troonie.PasswordDialog.Gtk.Container+ContainerChild
			vbox = new VBox ();
			vbox.Name = "vbox1";
			vbox.Spacing = 10;
			vbox.BorderWidth = ((uint)(10));
			// Container child vbox1.Gtk.Box+BoxChild
			entry = new Entry ();
			entry.CanFocus = true;
//			entryPassword.Name = "entryPassword";
			entry.IsEditable = true;
			entry.MaxLength = 30;
//			entry.Visibility = false;
//			entryPassword.InvisibleChar = '●';
			vbox.Add (entry);
			Box.BoxChild w1 = ((Box.BoxChild)(vbox [entry]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			btnOk = new TroonieButton ();
//			btnOk.Name = "btnOk";
			btnOk.CheckReleaseState = false;
			btnOk.BorderlineWidth = 3;
			btnOk.ButtonHeight = 35;
			btnOk.ButtonWidth = 0;
			btnOk.Font = "Arial";
			btnOk.Text = "OK";
			btnOk.TextSize = 14;
			vbox.Add (btnOk);
			Box.BoxChild w2 = ((Box.BoxChild)(vbox [btnOk]));
			w2.Position = 2;
			w2.Expand = false;
			w2.Fill = false;
			Add (vbox);
			if ((Child != null)) {
				Child.ShowAll ();
			}
			DefaultWidth = 348;
			DefaultHeight = 97;
			Show ();
			entry.KeyReleaseEvent += OnEntryKeyKeyReleaseEvent;
			btnOk.ButtonReleaseEvent += OnBtnOkReleaseEvent;

			SetEntryStartText ();

			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
		}

		private void SetEntryStartText()
		{
			if (tags == Tags.Keywords || tags == Tags.Composers){
				if (pressedInVIPs.Count == 1) {
					List<string> o = pressedInVIPs [0].TagsData.GetValue (tags) as List<string>;
					if (o != null && o.Count != 0) {
						foreach (string s in o) {
							entry.Text += s + ',';	
						}
					}
				} else {
					addMode = true;
				}

//				for (int i = 0; i < pressedInVIPs.Count; i++) {
//					ViewerImagePanel vip = pressedInVIPs [i];
////					string[] o = vip.TagsData.GetValue (tags) as string[];
//					List<string> o = vip.TagsData.GetValue (tags) as List<string>;
//
//					if (o != null && o.Count != 0) {
//						foreach (string s in o) {
//							entry.Text += s;	
//						}
//
//					}
//				}
			} else {
			}



			if (pressedInVIPs.Count == 1) {
				
			} else {
			}
		}			

		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (entry.Text.Length == 0) {
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.Title = Language.I.L [118];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [119];
				warn.OkButtontext = Language.I.L [16];
				warn.Show ();

				return;
			}

//			//fire the event now
//			if (OnReleasedOkButton != null) //is there a EventHandler?
//			{					
//				OnReleasedOkButton.Invoke(); //calls its EventHandler                
//			} //if not, ignore

			string[] elements = entry.Text.Split (',');
			for (int i = 0; i < elements.Length; i++) {
				elements [i] = elements [i].Trim ();
			}


			this.DestroyAll ();
		}

		protected void OnEntryKeyKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
//			if (entry.Text.Length == 0)
//				return;
//
//			char c = entry.Text [entry.Text.Length - 1];
//			if (c == ' ') {
//				entry.DeleteText (entry.CursorPosition - 1, entry.CursorPosition);
//			}

			if (args.Event.Key == Gdk.Key.Return) {
				OnBtnOkReleaseEvent (o, null);
			}
		}
	}
}

