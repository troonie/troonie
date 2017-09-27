using System;
using Gtk;
using Troonie_Lib;
using System.Diagnostics;

namespace Troonie
{
	public partial class OptionsWidget : Gtk.Window
	{
		private const int maxNr = 20000;
		private int nr;
		private bool subtract;

		private bool repeatTimeout;
		//		private Slider timeoutSlider;
		//		private Gdk.Key timeoutKey;
		//		private int timeoutRotateValue;
		private Stopwatch timeoutSw;

		public OptionsWidget () :
		base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			this.KeepAbove = true;
			hypertextlabelVideoplayer.InitDefaultValues ();
			hypertextlabelVideoplayer.FileChooserAction = FileChooserAction.Open;
			hypertextlabelVideoplayer.OnHyperTextLabelTextChanged += OnHyperTextLabelTextChanged;

			timeoutSw = new Stopwatch();

			#region set GUI color
			this.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			eventbox1.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			eventboxPage1.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			eventboxPage2.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			//			notebook1.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			#endregion set GUI color

			if (!Constants.I.WINDOWS) {
				scrolledwindow1.ShadowType = ShadowType.In;
			}

			#region set GUI language
			this.Title = Language.I.L[227];
			//			notebook1.Page[0].
			lbPage1.UseMarkup = true;
			lbPage1.LabelProp = "<b>" + Language.I.L[228] + "</b>";

			lbPage2.UseMarkup = true;
			lbPage2.LabelProp = "<b>" + Language.I.L [138] + "</b>";

			lbMaxSideLengthFilterImage.ModifyFg (StateType.Normal, ColorConverter.Instance.FONT);
			lbFrameVideoplayer.ModifyFg (StateType.Normal, ColorConverter.Instance.FONT);
			lbMaxSideLengthFilterImage.LabelProp = Constants.N + "<b>" + Language.I.L[284] + "</b>";
			lbFrameVideoplayer.LabelProp = Constants.N + "<b>" + Language.I.L[230] + "</b>";

			hypertextlabelVideoplayer.Text = Constants.I.CONFIG.VideoplayerPath; //Language.I.L[229];
			nr = Constants.I.CONFIG.MaxImageLengthForFiltering;
			entryMaxSideLengthFilterImage.Text = nr.ToString();

			#endregion set GUI language

			#region fill shortcuts table
			uint l_nr = 0;
			// converter shortcuts
			FillTable (Constants.N + Language.I.L [139], ref l_nr, true);
			FillTable (Language.I.L [140], ref l_nr, false);
			FillTable (Language.I.L [141], ref l_nr, false);
			FillTable (Language.I.L [142], ref l_nr, false);
			// editor shortcuts
			FillTable (Constants.N + Language.I.L [143], ref l_nr, true);
			FillTable (Language.I.L [281], ref l_nr, false);
			FillTable (Language.I.L [144], ref l_nr, false);
			FillTable (Language.I.L [283], ref l_nr, false);
			FillTable (Language.I.L [145], ref l_nr, false);
			FillTable (Language.I.L [146], ref l_nr, false);
			// steganography shortcuts
			FillTable (Constants.N + Language.I.L [147], ref l_nr, true);
			FillTable (Language.I.L [281], ref l_nr, false);
			FillTable (Language.I.L [148], ref l_nr, false);
			FillTable (Language.I.L [282], ref l_nr, false);
			// viewer shortcuts
			FillTable (Constants.N + Language.I.L [209], ref l_nr, true);
			FillTable (Language.I.L [210], ref l_nr, false);
			FillTable (Language.I.L [211], ref l_nr, false);
			FillTable (Language.I.L [212], ref l_nr, false);
			FillTable (Language.I.L [213], ref l_nr, false);
			FillTable (Language.I.L [219], ref l_nr, false);
			FillTable (Language.I.L [220], ref l_nr, false);
			FillTable (Language.I.L [221], ref l_nr, false);
			FillTable (Language.I.L [222], ref l_nr, false);
			FillTable (Language.I.L [223], ref l_nr, false);
			FillTable (Language.I.L [224], ref l_nr, false);
			FillTable (Language.I.L [225], ref l_nr, false);
			FillTable (Language.I.L [226], ref l_nr, false);
			// hidden
			FillTable (Language.I.L [214], ref l_nr, false);
			FillTable (Language.I.L [215], ref l_nr, false);
			FillTable (Language.I.L [216], ref l_nr, false);
			FillTable (Language.I.L [217], ref l_nr, false);
			#endregion fill shortcuts table
		}

		private void FillTable(string pText, ref uint l_nr, bool isOverview)
		{
			int lb1_width = 80;
			int lb2_width = 260;

			string[] s = pText.Split(new [] {": "}, StringSplitOptions.RemoveEmptyEntries);
			Fixed fixed1 = new Fixed();
			fixed1.WidthRequest = lb1_width;
			Label label1 = new Label();
			fixed1.Add (label1);
			Fixed.FixedChild fixedChild = (Fixed.FixedChild)fixed1[label1];
			fixedChild.X = 5;
			fixed1.Visible = true;
			label1.Visible = true;

			if (isOverview) {			
				label1.ModifyFg (StateType.Normal, ColorConverter.Instance.FONT);
				label1.UseMarkup = true;
				label1.LabelProp = "<b>" + s [0] + "</b>";
				table1.Attach (fixed1, 0, 1, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			} else {
//				label1.WidthRequest = lb1_width;
				label1.Text = s [0];				
				label1.TooltipText = s[0];
				table1.Attach (fixed1, 0, 1, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);

				Fixed fixed2 = new Fixed();
				fixed2.WidthRequest = lb2_width;
				Label label2 = new Label();

				const int LENGTH = 38;
				int n; // = s [1].IndexOf (' ', 15);
				if (s [1].Length > LENGTH && (n = s [1].IndexOf (' ', LENGTH)) != -1) {
					s [1] = s [1].Insert (n + 1, Constants.N);
				}
				label2.Text = s[1];
				label2.TooltipText = s[1];
				fixed2.Add (label2);
				fixedChild = (Fixed.FixedChild)fixed2[label2];
				fixedChild.X = 5;
				fixed2.Visible = true;
				label2.Visible = true;
				table1.Attach (fixed2, 1, 2, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			}

			l_nr++;
		}
			
		#region event handler

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();
		}

		protected void OnEntryMaxSideLengthFilterImageKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			int l_nr = 0;
			bool b = int.TryParse (entryMaxSideLengthFilterImage.Text, out l_nr);
			if (!b && entryMaxSideLengthFilterImage.Text.Length != 0) {
				entryMaxSideLengthFilterImage.DeleteText (
					entryMaxSideLengthFilterImage.CursorPosition - 1, entryMaxSideLengthFilterImage.CursorPosition);
			}

			if (l_nr > maxNr) {
				entryMaxSideLengthFilterImage.Text = maxNr.ToString();
			}

			int.TryParse (entryMaxSideLengthFilterImage.Text, out nr); 
			Constants.I.CONFIG.MaxImageLengthForFiltering = nr;
			Config.Save (Constants.I.CONFIG);
		}

		protected void OnButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			timeoutSw.Stop ();
			repeatTimeout = false;
		}

		protected void OnBtnMaxSideLengthFilterImagePressEvent (object o, ButtonPressEventArgs args)
		{
			TroonieButton tb = o as TroonieButton;
			subtract = tb != null && tb == btnMinusMaxSideLengthFilterImage;

			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			repeatTimeout = true;
			ChangeMaxImageLengthForFiltering ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(ChangeMaxImageLengthForFilteringTimeoutHandler));
		}

		#endregion event handler

		private void OnHyperTextLabelTextChanged()
		{
			Constants.I.CONFIG.VideoplayerPath = hypertextlabelVideoplayer.Text;			
			Constants.I.CONFIG.VideoplayerWorks = System.IO.File.Exists(hypertextlabelVideoplayer.Text);
			Config.Save (Constants.I.CONFIG);
		}

		private void ChangeMaxImageLengthForFiltering()
		{
			nr = subtract ? nr - 1 : nr + 1;

			if (nr < 0)
				nr = 0;
			else if (nr > maxNr)
				nr = maxNr;

			entryMaxSideLengthFilterImage.Text = nr.ToString();

			Constants.I.CONFIG.MaxImageLengthForFiltering = nr;			
			Config.Save (Constants.I.CONFIG);
		}

		private bool ChangeMaxImageLengthForFilteringTimeoutHandler()
		{
			if (timeoutSw.ElapsedMilliseconds < Constants.TIMEOUT_INTERVAL_FIRST) {
				return repeatTimeout;
			}

			ChangeMaxImageLengthForFiltering();
			return repeatTimeout;
		}
	}
}

