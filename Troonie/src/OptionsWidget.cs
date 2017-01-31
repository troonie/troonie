using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public partial class OptionsWidget : Gtk.Window
	{
		public OptionsWidget () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();

			this.KeepAbove = true;
			hypertextlabelVideoplayer.InitDefaultValues ();
			hypertextlabelVideoplayer.FileChooserAction = FileChooserAction.Open;
			hypertextlabelVideoplayer.OnHyperTextLabelTextChanged += OnHyperTextLabelTextChanged;

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
			#endregion set GUI language

		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();
		}

		private void OnHyperTextLabelTextChanged()
		{
			Constants.I.CONFIG.VideoplayerPath = hypertextlabelVideoplayer.Text;			
			Constants.I.CONFIG.VideoplayerWorks = System.IO.File.Exists(hypertextlabelVideoplayer.Text);
			Config.Save (Constants.I.CONFIG);
		}
	}
}

