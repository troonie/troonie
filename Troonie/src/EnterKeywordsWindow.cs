using System;
using Gtk;
using Troonie_Lib;
using System.Collections.Generic;
using Key = Gdk.Key;

namespace Troonie
{
	public enum SaveTagModeKeywords
	{
		/// <summary>Sets (and replaces) string array of Keywords tag. </summary>
		Set,
		/// <summary>Adds elements to string array of Keywords tag. </summary>
		Add,
	}

	public partial class EnterKeywordsWindow : Gtk.Window
	{
//		private const string MULTIVALUES = "####";
		bool existsSelectedRegion;
		private int selectRegionStartPosition, selectRegionEndPosition, indexSelectedKeyword;
		private string oldEntryText;
		private SaveTagModeKeywords saveTagMode2;
		private VBox vbox;
		private HBox hbox1, hbox2;
		private Entry entry;
		private CheckButton checkBtnDeleteKeywords;
		private TroonieButton btnOk, btnCancel;
		private List<ViewerImagePanel> pressedInVIPs;
		KeywordSerializer ks;

		public EnterKeywordsWindow (List<ViewerImagePanel> pressedInVIPs) : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			this.pressedInVIPs = pressedInVIPs;
			WindowPosition = Gtk.WindowPosition.CenterOnParent; 

			// deserialization
			ks = KeywordSerializer.Load ();
            if (ks.LoadError) {
                OkCancelDialog warn = new OkCancelDialog(true);
                warn.WindowPosition = WindowPosition.CenterAlways;
                warn.Title = "OI Title";  //Language.I.L[233];
                warn.Label1 = string.Empty;
                warn.Label2 = "OI Label2";  //Language.I.L[234];
                warn.OkButtontext = Language.I.L[16];
                warn.Show();
            }

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

//			entry.KeyPressEvent += OnEntryKeyPressEvent;
			entry.KeyReleaseEvent += OnEntryKeyReleaseEvent;
			btnOk.ButtonReleaseEvent += OnBtnOkReleaseEvent;
			btnCancel.ButtonReleaseEvent += (o, args) => { this.DestroyAll (); };

			SetEntryStartText ();

			if (saveTagMode2 == SaveTagModeKeywords.Add) {
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
			string errorImages = string.Empty;

			foreach (var vip in pressedInVIPs) {

				bool setValueSuccess = false;

				List<string> elements = new List<string>(entry.Text.Split (','));

				for (int i = 0; i < elements.Count; i++) {
					// trim all elements
					elements [i] = elements [i].Trim ();
					// make Uppercase
					if (elements [i].Length != 0) {
						elements [i] = elements [i].ToUpper () [0] + elements [i].Substring (1).ToLower ();
					}
				}		

				elements.RemoveAll (x => x == string.Empty);

				// remove duplicates
				HashSet<string> set1 = new HashSet<string>();
				elements.RemoveAll(x => !set1.Add(x));
				set1.Clear (); set1 = null;

				if (saveTagMode2 == SaveTagModeKeywords.Add) {

					List<string> keywordList = vip.TagsData.GetValue (TagsFlag.Keywords) as List<string>;
					if (keywordList == null) {
						keywordList = new List<string> ();
					}

					if (checkBtnDeleteKeywords.Active) {
						foreach (var s in elements) {
							keywordList.RemoveAll (x => x == s);	
						}
						AddKeywords (elements, -1, ks);

					} else {
						keywordList.AddRange (elements);
						AddKeywords (elements, 1, ks);
					}

					// remove duplicates
					HashSet<string> set2 = new HashSet<string>();
					keywordList.RemoveAll(x => !set2.Add(x));
					set2.Clear (); set2 = null;

					setValueSuccess = vip.TagsData.SetValue (TagsFlag.Keywords, keywordList);

				} else { // if (saveTagMode2 == SaveTagMode2.Set) {
					List<string> o = pressedInVIPs [0].TagsData.GetValue (TagsFlag.Keywords) as List<string>;

					// remove duplicates
					HashSet<string> set3 = new HashSet<string>();
					elements.RemoveAll(x => !set3.Add(x));
					set3.Clear (); set3 = null;

					List<string> elem2 = new List<string> (elements); // need deep copy here!
					setValueSuccess = vip.TagsData.SetValue (TagsFlag.Keywords, elements);

					if (o != null && o.Count != 0) {
						// remove all old keywords for xml serialization
						elem2.RemoveAll (x => o.Contains(x));
					}
					AddKeywords (elem2, 1, ks);
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
//						ShowErrorMessageWindow(TagsFlag.Keywords);
						errorImages += vip.RelativeImageName + Constants.N;
						break;
					}
				} else {
//					ShowErrorMessageWindow(TagsFlag.Keywords);
					errorImages += vip.RelativeImageName + Constants.N;
					break;
				}

			}	

			// sort and serialization
			ks.Keywords.Sort (new Keyword.ComparerDescendingByCountAndAscendingByText ());
			if (ks.Keywords.Count >= KeywordSerializer.MAX_NUMBER_OF_KEYWORDS) {
				int length = ks.Keywords.Count - KeywordSerializer.MAX_NUMBER_OF_KEYWORDS;
				ks.Keywords.RemoveRange (KeywordSerializer.MAX_NUMBER_OF_KEYWORDS, length);
			}
			KeywordSerializer.Save (ks);

			if (errorImages.Length != 0) {				
				ViewerWidget.ShowErrorDialog(Language.I.L [188] + Enum.GetName(typeof(TagsFlag), TagsFlag.Keywords) + Language.I.L [189], 
					errorImages + Constants.N);
			}

			this.DestroyAll ();
		}			
			
		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			SaveEntry ();
		}
			
		protected void OnEntryKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
//			entry.DeleteSelection (); // not necessary

			switch (args.Event.Key) {
			// keys without special behaviour
			case Key.Shift_L:
			case Key.Shift_R:
			case Key.Shift_Lock:
			case Key.Caps_Lock:
			case Key.Control_L:
			case Key.Control_R:
			case Key.Alt_L:
			case Key.Alt_R:
			case Key.space:
			case Key.End:
			case Key.Begin:
			case Key.Delete:
			case Key.Home:
			case Key.Insert:
			case Key.Page_Up:
			case Key.Page_Down:
			case Key.comma:
				return;
			case Key.Return:
				OnBtnOkReleaseEvent (o, null);
				return;
			case Key.Escape:
				this.DestroyAll ();
				return;
			case Key.BackSpace:
				if (existsSelectedRegion) {
					if (entry.Text.Length > selectRegionStartPosition - 1) {
						entry.Text = entry.Text.Substring (0, selectRegionStartPosition - 1);
						// just to set cursor to end
						entry.SelectRegion (entry.Text.Length, entry.Text.Length);
					}
				}
				oldEntryText = entry.Text;
				existsSelectedRegion = false;
				indexSelectedKeyword = 0;
				return;
			case Key.Left:
 				if (existsSelectedRegion) {
					if (entry.Text.Length > selectRegionStartPosition) {
						entry.Text = entry.Text.Substring (0, selectRegionStartPosition);
						// just to set cursor to end
						entry.SelectRegion(entry.Text.Length, entry.Text.Length);
					}
				}
				oldEntryText = entry.Text;
				existsSelectedRegion = false;
				indexSelectedKeyword = 0;
				return;
			case Key.Right:
			case Key.Tab:
				if (existsSelectedRegion) {
					string delimiter = ", "; 
					entry.Text += delimiter; 
					// just to set cursor to end
					entry.SelectRegion(entry.Text.Length, entry.Text.Length);
				}
				oldEntryText = entry.Text;
				existsSelectedRegion = false;
				indexSelectedKeyword = 0;
				return;
			case Key.Down:
				if (existsSelectedRegion) {
					indexSelectedKeyword++;
					entry.Text = oldEntryText;
				}
				break;
			case Key.Up:
				if (existsSelectedRegion) {
					if (indexSelectedKeyword != 0) {
						indexSelectedKeyword--;
					}
					entry.Text = oldEntryText;
				}
				break;
			}										


			#region entry preview
			List<string> elements = new List<string>(entry.Text.Split (','));

			for (int i = 0; i < elements.Count; i++) {
				// trim all elements
				elements [i] = elements [i].Trim ();
				// make Uppercase
				if (elements [i].Length != 0) {
					elements [i] = elements [i].ToUpper () [0] + elements [i].Substring (1).ToLower ();
				}
			}		
			elements.RemoveAll (x => x == string.Empty);

			if (elements.Count == 0) {
				return;
			}

			// remove duplicates
			HashSet<string> set = new HashSet<string>();
			elements.RemoveAll(x => !set.Add(x));
			set.Clear (); set = null;

			string lastElem = elements [elements.Count - 1];
			elements.Clear ();
			// ###

			if (lastElem.Length > 0)
			{
				List<Keyword> match = ks.Keywords.FindAll(x => x.Text.StartsWith(lastElem));
				if (indexSelectedKeyword >= match.Count) {
					indexSelectedKeyword = 0;
				}
					
				if (match.Count != 0) {
					oldEntryText = entry.Text;
					int start = entry.Text.ToLower().LastIndexOf(lastElem.ToLower());
					string bin = match[indexSelectedKeyword].Text;
					entry.Text = entry.Text.Remove(start) + bin;
					existsSelectedRegion = true;
					selectRegionStartPosition = start + lastElem.Length;
					selectRegionEndPosition = entry.Text.Length;
					entry.SelectRegion (selectRegionStartPosition, selectRegionEndPosition);
				}
				else {
					oldEntryText = entry.Text;
					existsSelectedRegion = false;
					indexSelectedKeyword = 0;
				}					
			}

			#endregion entry preview
		}

		#region static helper function

		private static void AddKeywords(List<string> keywords, int add, KeywordSerializer keywordSerializer) 
		{
			// adding new keywords to xml
			foreach (string s in keywords) {
				Keyword key1 = new Keyword(s);
				if (keywordSerializer.Keywords.Contains (key1)) {
					int n = keywordSerializer.Keywords.IndexOf (key1);
					keywordSerializer.Keywords [n].Count+= add;
				} else {
					if (add == 1) keywordSerializer.Keywords.Add (key1);
					else keywordSerializer.Keywords.Remove (key1);
				}
			}				
		}

//		private static void ShowErrorMessageWindow(TagsFlag flag)
//		{
//			OkCancelDialog info = new OkCancelDialog (true);
//			info.WindowPosition = WindowPosition.CenterAlways;
//			info.Title = Language.I.L [153];
//			info.Label1 = Language.I.L [188] + Enum.GetName(typeof(TagsFlag), flag) + Language.I.L [189];
//			info.Label2 = Language.I.L [190];
//			info.OkButtontext = Language.I.L [16];
//			info.Show ();
//		}

		public static string SetStartText(List<ViewerImagePanel> pressedInVIPs, out SaveTagModeKeywords saveTagMode)
		{
			string startText = string.Empty;
			saveTagMode = SaveTagModeKeywords.Set; 
			if (pressedInVIPs.Count == 0)
				return startText;

			if (pressedInVIPs.Count == 1) {
				List<string> o = pressedInVIPs [0].TagsData.GetValue (TagsFlag.Keywords) as List<string>;
				if (o != null && o.Count != 0) {
					foreach (string s in o) {
						startText += s + ',';	
					}

					startText = startText.Substring (0, startText.Length - 1);
				}

//				saveTagMode = SaveTagMode2.Set; // not necessary to set again here

			} else {
				saveTagMode = SaveTagModeKeywords.Add;
			}

			return startText;
		}		

		#endregion
	}
}

