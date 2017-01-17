using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;

namespace Troonie
{
	public enum SaveTagMode
	{
		/// <summary> No image is enabled. </summary>
		Nothing,
		/// <summary>Replaces string array of tag. Only for tags with multiple values (Keywords, Composers). </summary>
		replaceStringarray,
		/// <summary>Adds an element to string array of tag. Only for tags with multiple values (Keywords, Composers). </summary>
		addToStringarray,
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
		private TroonieButton btnOk, btnCancel;
		private TagsFlag tags;
		private List<ViewerImagePanel> pressedInVIPs;


		public EnterMetaDataWindow (List<ViewerImagePanel> pressedInVIPs, TagsFlag tags) : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			this.tags = tags;
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


			hbox = new HBox ();
			hbox.Spacing = 6;

			btnOk = new TroonieButton ();
//			btnOk.Name = "btnOk";
			btnOk.CheckReleaseState = false;
			btnOk.BorderlineWidth = 3;
			btnOk.ButtonHeight = 35;
			btnOk.ButtonWidth = 0;
			btnOk.Font = "Arial";
			btnOk.TextSize = 14;
			btnOk.Text = Language.I.L [16];
			hbox.Add (btnOk);
			Box.BoxChild hbox_w1 = (Box.BoxChild)(hbox [btnOk]);
			hbox_w1.Position = 0;
			hbox_w1.Padding = 4;

			btnCancel = new TroonieButton ();
			btnCancel.CheckReleaseState = false;
			btnCancel.BorderlineWidth = 3;
			btnCancel.ButtonHeight = 35;
			btnCancel.ButtonWidth = 0;
			btnCancel.Font = "Arial";
			btnCancel.TextSize = 14;
			btnCancel.Text = Language.I.L [17];
			hbox.Add (btnCancel);
			Box.BoxChild hbox_w2 = (Box.BoxChild)(hbox [btnCancel]);
			hbox_w2.Position = 1;
			hbox_w2.Padding = 4;

			vbox.Add (hbox);
			Box.BoxChild w2 = ((Box.BoxChild)(vbox [hbox]));
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
			btnCancel.ButtonReleaseEvent += (o, args) => { this.DestroyAll (); };

			SetEntryStartText ();
			switch (saveTagMode) 
			{
			case SaveTagMode.addToStringarray:
				Title =  Language.I.L [182] + Enum.GetName(typeof(TagsFlag), tags) + Language.I.L [187];
				break;
			case SaveTagMode.replaceStringarray:
			case SaveTagMode.setValueInOneTag:
				Title = Language.I.L [183] + Enum.GetName (typeof(TagsFlag), tags) + Language.I.L [187];
				break;
			case SaveTagMode.replaceValueInSeveralTags:
				Title = Language.I.L [184] + Enum.GetName (typeof(TagsFlag), tags) + Language.I.L [187];
				break;
			}


			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
		}

		private void SetEntryStartText()
		{
			entry.Text = SetStartText (pressedInVIPs, tags, out saveTagMode);
		}

		private void SaveEntry()
		{
			foreach (var vip in pressedInVIPs) {

				bool setValueSuccess = false;
				switch (tags) {
				case TagsFlag.Altitude:			
				case TagsFlag.Creator:	
				case TagsFlag.DateTime:			
				case TagsFlag.ExposureTime:	
				case TagsFlag.FNumber:				
				case TagsFlag.FocalLength:	
				case TagsFlag.FocalLengthIn35mmFilm:				
				case TagsFlag.ISOSpeedRatings:	
				case TagsFlag.Latitude:				
				case TagsFlag.Longitude:			
				case TagsFlag.Make:						
				case TagsFlag.Model:				
				case TagsFlag.Orientation:		
				case TagsFlag.Rating:				
				case TagsFlag.Software:			
					// other tags
				case TagsFlag.Comment:					
				case TagsFlag.Conductor:		
				case TagsFlag.Copyright:			
				case TagsFlag.Title:			
				case TagsFlag.Track:
				case TagsFlag.TrackCount:
				case TagsFlag.Year:
					setValueSuccess = vip.TagsData.SetValue (tags, entry.Text);
					break;

				case TagsFlag.Keywords:
				case TagsFlag.Composers:
					string[] elements = entry.Text.Split (',');
					for (int i = 0; i < elements.Length; i++) {
						elements [i] = elements [i].Trim ();
					}

					if (elements.Length == 0) {
						break;
					}

					if (saveTagMode == SaveTagMode.addToStringarray) {
						List<string> keywordList = vip.TagsData.GetValue (tags) as List<string>;
						if (keywordList == null) {
							keywordList = new List<string> ();
						}

						keywordList.AddRange (elements);
						setValueSuccess = vip.TagsData.SetValue (tags, keywordList);
					} else if (saveTagMode == SaveTagMode.replaceStringarray) {
						setValueSuccess = vip.TagsData.SetValue (tags, elements);
					} else {
						// SHOULD/CAN NOT BE HAPPENED
					}
					break;
					//					break;
					//					default:
					//						return false;
				}

				if (setValueSuccess) {
					bool success = vip.IsVideo ? 
						VideoTagHelper.SetTag (vip.OriginalImageFullName, tags, vip.TagsData) :
						ImageTagHelper.SetTag (vip.OriginalImageFullName, tags, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
						// dirty workaround to refresh label strings of ViewerWidget.tableTagsViewer
						vip.IsPressedIn = vip.IsPressedIn;
					} else {
						ShowErrorMessageWindow(tags);
						break;
					}
				} else {
					ShowErrorMessageWindow(tags);
					break;
				}

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

			// if (saveTagMode == SaveTagMode.addToStringarray || saveTagMode == SaveTagMode.replaceStringarray) {
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

		public static string SetStartText(List<ViewerImagePanel> pressedInVIPs, TagsFlag tags, out SaveTagMode saveTagMode)
		{
			string startText = string.Empty;
			saveTagMode = SaveTagMode.Nothing;
			if (pressedInVIPs.Count == 0)
				return startText;

			// multiple values in tag (Keywords, Composers)
			if (tags == TagsFlag.Keywords || tags == TagsFlag.Composers){
				if (pressedInVIPs.Count == 1) {
					List<string> o = pressedInVIPs [0].TagsData.GetValue (tags) as List<string>;
					if (o != null && o.Count != 0) {
						foreach (string s in o) {
							startText += s + ',';	
						}
					}

					saveTagMode = SaveTagMode.replaceStringarray; 
				} else {
					saveTagMode = SaveTagMode.addToStringarray; //addMode = true;
				}
			} 
			// (other) single tags
			else {
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

					saveTagMode = SaveTagMode.setValueInOneTag;

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
			}

			return startText;
		}		

		#endregion
	}
}

