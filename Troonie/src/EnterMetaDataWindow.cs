using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;

namespace Troonie
{
	public enum SaveTagMode
	{
		/// <summary>Replaces value in several tags. Not for tags with multiple values (Keywords, Composers).</summary>
		replaceValueInSeveralTags,
		/// <summary>Sets or replaces the value of one tag. Not for tags with multiple values (Keywords, Composers).</summary>
		setValueInOneTag
	}

	public partial class EnterMetaDataWindow : Gtk.Window
	{
		private const string MULTIVALUES = "####";
		private SaveTagMode saveTagMode;
		private VBox vbox;
		private HBox hbox;
		private Entry entry;
		private ComboBox combobox;
		private TroonieButton btnOk, btnCancel;
		private TagsFlag tags;
		private List<ViewerImagePanel> pressedInVIPs;

		private List<int> comboboxIds;

		public EnterMetaDataWindow (List<ViewerImagePanel> pressedInVIPs, TagsFlag tags) : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			this.tags = tags;
			this.pressedInVIPs = pressedInVIPs;

			WindowPosition = Gtk.WindowPosition.CenterOnParent; 

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

			// 1st parameter for horicontal, 2nd for vertical
			// left/top = 0.0f
			// center = 0.5f
			// right/bottom = 1.0f
			Alignment a = new Alignment (0.0f, 0, 0.3f, 0);
			combobox = new ComboBox(new[]{"-"});
			//			lbInfo.Label = Language.I.L [202];
			a.Add(combobox);

			vbox.Add (a);
			Box.BoxChild w2 = ((Box.BoxChild)(vbox [a]));
			w2.Position = 1;
			w2.Expand = false;
			w2.Fill = false;

			hbox = new HBox ();
			hbox.Spacing = 6;

			btnOk = new TroonieButton ();
			btnOk.CheckReleaseState = false;
			btnOk.BorderlineWidth = 3;
			btnOk.ButtonHeight = 35;
			btnOk.ButtonWidth = 0;
			btnOk.Font = "Arial";
			btnOk.TextSize = 14;
			btnOk.Text = Language.I.L [16];
			hbox.Add (btnOk);
			Box.BoxChild hbox_w2a = (Box.BoxChild)(hbox [btnOk]);
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
			hbox.Add (btnCancel);
			Box.BoxChild hbox_w2b = (Box.BoxChild)(hbox [btnCancel]);
			hbox_w2b.Position = 1;
			hbox_w2b.Padding = 4;

			vbox.Add (hbox);
			Box.BoxChild w3 = ((Box.BoxChild)(vbox [hbox]));
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
			SetInfoLabel ();
			Title = saveTagMode == SaveTagMode.setValueInOneTag ? Language.I.L [183] : Language.I.L [184];
			Title += Enum.GetName (typeof(TagsFlag), tags) + Language.I.L [187];

			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
		}

		private void SetInfoLabel()
		{
			combobox.Visible = false;
			combobox.RemoveText (0);

			switch (tags) {
			case TagsFlag.Flash:
				// source: http://www.awaresystems.be/imaging/tiff/tifftags/privateifd/exif/flash.html
//				lbInfo.Text = 
				combobox.AppendText ("0 = Flash did not fire ");
				combobox.AppendText ("1 = Flash fired	");
				combobox.AppendText ("5 = Strobe return light not detected ");
				combobox.AppendText ("7 = Strobe return light detected ");
				combobox.AppendText ("9 = Flash fired, compulsory flash mode ");
				combobox.AppendText ("13 = Flash fired, compulsory flash mode, return light not detected ");
				combobox.AppendText ("15 = Flash fired, compulsory flash mode, return light detected ");
				combobox.AppendText ("16 = Flash did not fire, compulsory flash mode ");
				combobox.AppendText ("24 = Flash did not fire, auto mode ");
				combobox.AppendText ("25 = Flash fired, auto mode	");
				combobox.AppendText ("29 = Flash fired, auto mode, return light not detected ");
				combobox.AppendText ("31 = Flash fired, auto mode, return light detected ");
				combobox.AppendText ("32 = No flash function ");
				combobox.AppendText ("65 = Flash fired, red-eye reduction mode	");
				combobox.AppendText ("69 = Flash fired, red-eye reduction mode, return light not detected ");
				combobox.AppendText ("71 = Flash fired, red-eye reduction mode, return light detected ");
				combobox.AppendText ("73 = Flash fired, compulsory flash mode, red-eye reduction mode ");
				combobox.AppendText ("77 = Flash fired, compulsory flash mode, red-eye reduction mode, return light not detected ");
				combobox.AppendText ("79 = Flash fired, compulsory flash mode, red-eye reduction mode, return light detected ");
				combobox.AppendText ("89 = Flash fired, auto mode, red-eye reduction mode ");
				combobox.AppendText ("93 = Flash fired, auto mode, return light not detected, red-eye reduction mode ");
				combobox.AppendText ("95 = Flash fired, auto mode, return light detected, red-eye reduction mode ");
				comboboxIds = new List<int> { 0, 1, 5, 7, 9, 13, 15, 16, 24, 25, 29, 31, 32, 65, 69, 71, 73, 77, 79, 89, 93, 95 };
				InitCombobox ();
				break;
			case TagsFlag.MeteringMode:
				combobox.AppendText ("0 = Unknown ");
				combobox.AppendText ("1 = Average ");
				combobox.AppendText ("2 = CenterWeightedAverage ");
				combobox.AppendText ("3 = Spot ");
				combobox.AppendText ("4 = MultiSpot ");
				combobox.AppendText ("5 = Pattern ");
				combobox.AppendText ("6 = Partial ");
				combobox.AppendText ("255 = Other ");
				comboboxIds = new List<int> { 0, 1, 2, 3, 4, 5, 6, 255 };
				InitCombobox ();
				break;
			case TagsFlag.Orientation:						
				combobox.AppendText ("0 = " + Language.I.L [203]);
				combobox.AppendText ("1 = " + Language.I.L [204]);
				combobox.AppendText ("3 = " + Language.I.L [205]);
				combobox.AppendText ("6 = " + Language.I.L [206]);
				combobox.AppendText ("8 = " + Language.I.L [207]);
				comboboxIds = new List<int> { 0, 1, 3, 6, 8 };
				InitCombobox ();
				break;
			default:
				break;
			}				
		}

		private void InitCombobox()
		{
			entry.IsEditable = false;
			combobox.Visible = true;
			combobox.Changed +=	(sender, e) => { entry.Text = comboboxIds[combobox.Active].ToString(); };
			int tmp;
			if (int.TryParse (entry.Text, out tmp)) {
				combobox.Active = comboboxIds.IndexOf(tmp);
			}
		}

		private void SetEntryStartText()
		{
			entry.Text = SetStartText (pressedInVIPs, tags, out saveTagMode);
		}

		private void SaveEntry()
		{
			string errorImages = string.Empty;

			foreach (var vip in pressedInVIPs) {

				TagsData oldTagsData = vip.TagsData;
				bool setValueSuccess = vip.TagsData.SetValue (tags, entry.Text);

				if (setValueSuccess) {
					bool success = vip.IsVideo ? 
						VideoTagHelper.SetTag (vip.OriginalImageFullName, tags, vip.TagsData) :
						ImageTagHelper.SetTag (vip.OriginalImageFullName, tags, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
						// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
						vip.IsPressedIn = vip.IsPressedIn;
					} else {
						errorImages += vip.RelativeImageName + Constants.N;
					}
				} else {
					errorImages += vip.RelativeImageName + Constants.N;
				}

			}

			if (errorImages.Length != 0) {				
				ViewerWidget.ShowErrorDialog(Language.I.L [188] + Enum.GetName(typeof(TagsFlag), tags) + Language.I.L [189], 
											 errorImages + Constants.N);
			}

			this.DestroyAll ();
		}			
			
		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (entry.Text == MULTIVALUES) {
				OkCancelDialog info = new OkCancelDialog (true);
				info.WindowPosition = WindowPosition.CenterAlways;
				info.Title = Language.I.L [29];
				info.Label1 = Language.I.L [185];
				info.Label2 = Language.I.L [186];
				info.OkButtontext = Language.I.L [16];
				info.Show ();
				return;
			}
				
			if (saveTagMode != SaveTagMode.replaceValueInSeveralTags) {
				SaveEntry ();
				return;
			}

			OkCancelDialog warn = new OkCancelDialog (false);
			warn.WindowPosition = WindowPosition.CenterAlways;
			warn.Title = Language.I.L [29];
			warn.Label1 = Language.I.L [180] + Enum.GetName(typeof(TagsFlag), tags) + Language.I.L [181];
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

		public static string SetStartText(List<ViewerImagePanel> pressedInVIPs, TagsFlag tags, out SaveTagMode saveTagMode)
		{
			string startText = string.Empty;
			saveTagMode = SaveTagMode.setValueInOneTag;
			if (pressedInVIPs.Count == 0)
				return startText;
			
			object o = pressedInVIPs [0].TagsData.GetValue (tags);
			String so;
			if (o == null) {
				so = string.Empty;
			} else {
				so = o.ToString();
			}

			if (pressedInVIPs.Count == 1) {					
				if (so != null) {
					startText = so;
				}

//				saveTagMode = SaveTagMode.setValueInOneTag;

			} else {
				for (int i = 1; i < pressedInVIPs.Count; i++) {
					ViewerImagePanel vip = pressedInVIPs [i];
					o = vip.TagsData.GetValue (tags);
					String so2;
					if (o == null) {
						so2 = string.Empty;
					} else {
						so2 = o.ToString();
					}

					if (so != null && so2 != null && so == so2) {
						startText = so;
					} else if (so != null && so2 != null) {
						startText = MULTIVALUES;
						break;
					} else {
						startText = string.Empty;
					}
				}

				saveTagMode = SaveTagMode.replaceValueInSeveralTags;
			}

			return startText;
		}		

		#endregion
	}
}

