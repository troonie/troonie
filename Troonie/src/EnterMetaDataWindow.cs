using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;

namespace Troonie
{
	public partial class EnterMetaDataWindow : Gtk.Window
	{
		private bool addMode;
		private VBox vbox;
		private Entry entry;
		private TroonieButton btnOk;
		private TagsFlag tags;
		private List<ViewerImagePanel> pressedInVIPs;

		public String OkButtontext
		{
			get { return btnOk.Text; }
			set { btnOk.Text = value; }
		}

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
			entry.Text = SetStartText (pressedInVIPs, tags, ref addMode);
		}
			
		// TODO: Complete function.
		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{

			foreach (var vip in pressedInVIPs) {

				bool setValueSuccess = false;
				switch (tags) {
				case TagsFlag.Altitude:			
				case TagsFlag.Creator:	
//				case TagsFlag.DateTime:		return DateTime;		
				case TagsFlag.ExposureTime:	
				case TagsFlag.FNumber:				
				case TagsFlag.FocalLength:	
				case TagsFlag.FocalLengthIn35mmFilm:				
				case TagsFlag.ISOSpeedRatings:	
				case TagsFlag.Latitude:				
				case TagsFlag.Longitude:			
				case TagsFlag.Make:						
				case TagsFlag.Model:				
	//			case TagsFlag.Orientation:	return Orientation;		
				case TagsFlag.Rating:				
				case TagsFlag.Software:			
					// other tags
				case TagsFlag.Comment:					
				case TagsFlag.Conductor:		
				case TagsFlag.Copyright:			
				case TagsFlag.Title:			
//				case TagsFlag.Track:		return Track;			
//				case TagsFlag.TrackCount:	return TrackCount;		
//				case TagsFlag.Year:			return Year;
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

					if (addMode) {
						List<string> keywordList = vip.TagsData.GetValue (tags) as List<string>;
						if (keywordList == null) {
							keywordList = new List<string> ();
						}

						keywordList.AddRange (elements);
						setValueSuccess = vip.TagsData.SetValue (tags, keywordList);
					} else {
						setValueSuccess = vip.TagsData.SetValue (tags, elements);
					}
					break;
//				case TagsFlag.Comment:
//					vip.TagsData.Comment = entry.Text;
//					break;
//					default:
//						return false;
				}

				if (setValueSuccess) {
					bool success = ImageTagHelper.SetTag (vip.OriginalImageFullName, tags, vip.TagsData);
					if (success) {
						vip.QueueDraw ();
					} else {
						// TODO: Info, if failed.
					}
				} else {
					// TODO: Info, if failed.
				}

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

		#region public static helper function

		public static string SetStartText(List<ViewerImagePanel> pressedInVIPs, TagsFlag tags, ref bool addMode)
		{
			string startText = string.Empty;
			if (pressedInVIPs.Count == 0)
				return startText;

			// multiple tags (Keywords, Composers)
			if (tags == TagsFlag.Keywords || tags == TagsFlag.Composers){
				if (pressedInVIPs.Count == 1) {
					List<string> o = pressedInVIPs [0].TagsData.GetValue (tags) as List<string>;
					if (o != null && o.Count != 0) {
						foreach (string s in o) {
							startText += s + ',';	
						}
					}
				} else {
					addMode = true;
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
							startText = "####";
							break;
						} else {
							startText = string.Empty;
						}
					}
				}
			}

			return startText;
		}		

		#endregion
	}
}

