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
			lbPage2.LabelProp = "<b>" + "   " + "</b>";

			lbFrameVideoplayer.LabelProp = "<b>" + Language.I.L[230] + "</b>";

			hypertextlabelVideoplayer.Text = Constants.I.CONFIG.VideoplayerPath; //Language.I.L[229];
			nr = Constants.I.CONFIG.MaxImageLengthForFiltering;
			entryMaxSideLengthFilterImage.Text = nr.ToString();

			#endregion set GUI language

			SetTable1 ();
		}

		private void SetTable1()
		{
			uint l_nr = 0;
			int lb1_width = 80;
			int lb2_width = 180;

			string[] s = Language.I.L [140].Split(new [] {": "}, StringSplitOptions.RemoveEmptyEntries);
			Fixed fixed1 = new Fixed();
			fixed1.WidthRequest = lb1_width;
			Label label1 = new Label(s[0]);
			label1.TooltipText = s[0];
			fixed1.Add (label1);
			Fixed.FixedChild w14 = (Fixed.FixedChild)(fixed1 [label1]);
			w14.X = 5;

			Fixed fixed2 = new Fixed();
			fixed2.WidthRequest = lb2_width;
			Label label2 = new Label();
			label2.Text = s[1];
			label2.TooltipText = s[1];
			fixed2.Add (label2);
			table1.Attach (fixed1, 0, 1, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			table1.Attach (fixed2, 1, 2, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			l_nr++;

			s = Language.I.L [141].Split(new [] {": "}, StringSplitOptions.RemoveEmptyEntries);
			fixed1 = new Fixed();
			fixed1.WidthRequest = lb1_width;
			label1 = new Label(s[0]);
			label1.TooltipText = s[0];
			fixed1.Add (label1);
			fixed2 = new Fixed();
			fixed2.WidthRequest = lb2_width;
			label2 = new Label();
			label2.Text = s[1];
			label2.TooltipText = s[1];
			fixed2.Add (label2);
			table1.Attach (fixed1, 0, 1, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			table1.Attach (fixed2, 1, 2, l_nr, l_nr + 1, AttachOptions.Shrink, AttachOptions.Shrink, 0, 0);
			l_nr++;


			foreach (Widget w in table1.AllChildren) {
				w.Visible = true;
				w.ChildVisible = true;

				Fixed f = w as Fixed;
				if (f != null) {
					foreach (Widget w2 in f.AllChildren) {
						w2.Visible = true;
						w2.ChildVisible = true;		
					}
				}
			}
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

