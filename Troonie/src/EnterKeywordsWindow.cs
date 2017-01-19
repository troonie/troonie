using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;

namespace Troonie
{
	public enum SaveTagMode2
	{
		/// <summary>Replaces string array of tag. Only for tags with multiple values (Keywords, Composers). </summary>
		replaceStringarray,
		/// <summary>Adds an element to string array of tag. Only for tags with multiple values (Keywords, Composers). </summary>
		addToStringarray,
	}

	public partial class EnterKeywordsWindow : Gtk.Window
	{
//		private const string MULTIVALUES = "####";
		private SaveTagMode2 saveTagMode2;
		private VBox vbox;
		private HBox hbox1, hbox2;
		private Entry entry;
		private CheckButton checkBtnDeleteKeywords;
		private TroonieButton btnOk, btnCancel;
		private List<ViewerImagePanel> pressedInVIPs;


		public EnterKeywordsWindow (List<ViewerImagePanel> pressedInVIPs) : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			this.pressedInVIPs = pressedInVIPs;

			WindowPosition = Gtk.WindowPosition.CenterOnParent; // ((WindowPosition)(4));
			// Container child Troonie.PasswordDialog.Gtk.Container+ContainerChild
			vbox = new VBox ();
			vbox.Name = "vbox1";
			vbox.Spacing = 10;
			vbox.BorderWidth = 10;
			// Container child vbox1.Gtk.Box+BoxChild
			entry = new Entry ();
			entry.CanFocus = true;
//			entryPassword.Name = "entryPassword";
			entry.IsEditable = true;
			entry.MaxLength = 150;
//			entry.Visibility = false;
//			entryPassword.InvisibleChar = '●';
			vbox.Add (entry);
			Box.BoxChild w1 = ((Box.BoxChild)(vbox [entry]));
			w1.Position = 0;
			w1.Expand = false;
			w1.Fill = false;

			// hbox1 (incl. lbDummy and checkBtnDeleteKeywords) --> 2. element for vbox
			hbox1 = new HBox ();
			hbox1.Spacing = 6;

			checkBtnDeleteKeywords = new CheckButton();
			checkBtnDeleteKeywords.CanFocus = true;
			checkBtnDeleteKeywords.Label = Language.I.L [202];
			hbox1.Add (checkBtnDeleteKeywords);
			Box.BoxChild hbox_w1a = (Box.BoxChild)(hbox1 [checkBtnDeleteKeywords]);
			hbox_w1a.Position = 0;
			hbox_w1a.Padding = 4;

			Label lbDummy = new Label (string.Empty);
			hbox1.Add (lbDummy);
			Box.BoxChild hbox_w1b = (Box.BoxChild)(hbox1 [lbDummy]);
			hbox_w1b.Position = 1;
			hbox_w1b.Padding = 4;

			vbox.Add (hbox1);


			Box.BoxChild w2 = ((Box.BoxChild)(vbox [hbox1]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;


			hbox2 = new HBox ();
			hbox2.Spacing = 6;

			btnOk = new TroonieButton ();
//			btnOk.Name = "btnOk";
			btnOk.CheckReleaseState = false;
			btnOk.BorderlineWidth = 3;
			btnOk.ButtonHeight = 35;
			btnOk.ButtonWidth = 0;
			btnOk.Font = "Arial";
			btnOk.TextSize = 14;
			btnOk.Text = Language.I.L [16];
			hbox2.Add (btnOk);
			Box.BoxChild hbox_w2a = (Box.BoxChild)(hbox2 [btnOk]);
			hbox_w2a.Position = 0;
			hbox_w2a.Padding = 4;

			btnCancel = new TroonieButton ();
			btnCancel.CheckReleaseState = false;
			btnCancel.BorderlineWidth = 3;
			btnCancel.ButtonHeight = 35;
			btnCancel.ButtonWidth = 0;
			btnCancel.Font = "Arial";
			btnCancel.TextSize = 14;
			btnCancel.Text = Language.I.L [17];
			hbox2.Add (btnCancel);
			Box.BoxChild hbox_w2b = (Box.BoxChild)(hbox2 [btnCancel]);
			hbox_w2b.Position = 1;
			hbox_w2b.Padding = 4;

			vbox.Add (hbox2);
			Box.BoxChild w3 = ((Box.BoxChild)(vbox [hbox2]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			Add (vbox);

			if ((Child != null)) {
				Child.ShowAll ();
			}
			DefaultWidth = 348;
			DefaultHeight = 97;


			Show ();

			entry.KeyReleaseEvent += OnEntryKeyKeyReleaseEvent;
			btnOk.ButtonReleaseEvent += OnBtnOkReleaseEvent;
			btnCancel.ButtonReleaseEvent += (o, args) => { this.DestroyAll (); };

			SetEntryStartText ();

			if (saveTagMode2 == SaveTagMode2.addToStringarray) {
				Title = Language.I.L [182];
				checkBtnDeleteKeywords.Visible = true;
			} else {
				Title = Language.I.L [183];
				checkBtnDeleteKeywords.Visible = false;
			}				
			Title += Enum.GetName (typeof(TagsFlag), TagsFlag.Keywords) + Language.I.L [187];

			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
		}

		private void SetEntryStartText()
		{
			entry.Text = SetStartText (pressedInVIPs, out saveTagMode2);
		}

		private void SaveEntry()
		{
			foreach (var vip in pressedInVIPs) {

				bool setValueSuccess = false;

				string[] elements = entry.Text.Split (',');
				for (int i = 0; i < elements.Length; i++) {
					elements [i] = elements [i].Trim ();
				}

				if (elements.Length == 0) {
					break;
				}

				if (saveTagMode2 == SaveTagMode2.addToStringarray) {

					List<string> keywordList = vip.TagsData.GetValue (TagsFlag.Keywords) as List<string>;
					if (keywordList == null) {
						keywordList = new List<string> ();
					}

					if (checkBtnDeleteKeywords.Active) {
						foreach (var s in elements) {
							keywordList.RemoveAll (x => x == s);	
						}

					} else {
						keywordList.AddRange (elements);
					}

					setValueSuccess = vip.TagsData.SetValue (TagsFlag.Keywords, keywordList);

				} else { // if (saveTagMode2 == SaveTagMode2.replaceStringarray) {
					setValueSuccess = vip.TagsData.SetValue (TagsFlag.Keywords, elements);
				}

				if (setValueSuccess) {
					bool success = vip.IsVideo ? 
						VideoTagHelper.SetTag (vip.OriginalImageFullName, TagsFlag.Keywords, vip.TagsData) :
						ImageTagHelper.SetTag (vip.OriginalImageFullName, TagsFlag.Keywords, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
						// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
						vip.IsPressedIn = vip.IsPressedIn;
					} else {
						ShowErrorMessageWindow(TagsFlag.Keywords);
						break;
					}
				} else {
					ShowErrorMessageWindow(TagsFlag.Keywords);
					break;
				}

			}			

			this.DestroyAll ();
		}			
			
		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (saveTagMode2 == SaveTagMode2.addToStringarray) {
				SaveEntry ();
				return;
			}

			OkCancelDialog warn = new OkCancelDialog (false);
			warn.WindowPosition = WindowPosition.CenterAlways;
			warn.Title = Language.I.L [29];
			warn.Label1 = Language.I.L [180] + Enum.GetName(typeof(TagsFlag), TagsFlag.Keywords) + Language.I.L [181];
			warn.Label2 = Language.I.L [171];
			warn.OkButtontext = Language.I.L [16];
			warn.CancelButtontext = Language.I.L [17];	
			warn.Show ();

			warn.OnReleasedOkButton += SaveEntry;
		}

		protected void OnEntryKeyKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Return) {
				OnBtnOkReleaseEvent (o, null);
			}
			else if (args.Event.Key == Gdk.Key.Escape) {
				this.DestroyAll ();
			}
		}

		#region static helper function

		private static void ShowErrorMessageWindow(TagsFlag flag)
		{
			OkCancelDialog info = new OkCancelDialog (true);
			info.WindowPosition = WindowPosition.CenterAlways;
			info.Title = Language.I.L [153];
			info.Label1 = Language.I.L [188] + Enum.GetName(typeof(TagsFlag), flag) + Language.I.L [189];
			info.Label2 = Language.I.L [190];
			info.OkButtontext = Language.I.L [16];
			info.Show ();
		}

		public static string SetStartText(List<ViewerImagePanel> pressedInVIPs, out SaveTagMode2 saveTagMode)
		{
			string startText = string.Empty;
			saveTagMode = SaveTagMode2.replaceStringarray; 
			if (pressedInVIPs.Count == 0)
				return startText;

			if (pressedInVIPs.Count == 1) {
				List<string> o = pressedInVIPs [0].TagsData.GetValue (TagsFlag.Keywords) as List<string>;
				if (o != null && o.Count != 0) {
					foreach (string s in o) {
						startText += s + ',';	
					}
				}

//				saveTagMode = SaveTagMode2.replaceStringarray; 
			} else {
				saveTagMode = SaveTagMode2.addToStringarray;
			}

			return startText;
		}		

		#endregion
	}
}

