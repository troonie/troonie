using System;
using Gtk;
using Picturez_Lib;
using System.Diagnostics;

namespace Picturez
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StarterWidget : Gtk.Window
	{
		string[] args;
		string lastArg;

		public StarterWidget (string[] args) : base (Gtk.WindowType.Toplevel)
		{		
			this.args = args;
			if (args.Length != 0) {
				lastArg = args [args.Length - 1];
			}

			this.Build ();
			this.KeepAbove = true;
			this.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);

			picBtnConvert.Text = Language.I.L [67];
			picBtnEdit.Text = Language.I.L [68];
			picBtnSteganography.Text = Language.I.L [80];
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			this.DestroyAll ();

			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnPicBtnConvertButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
//			StartProcess (" -c ");
			StartProcess (0);
		}

		protected void OnPicBtnEditButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//			StartProcess (" -e ");
			StartProcess (1);
		}

		protected void OnPicBtnSteganographyButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//			StartProcess (" -s ");
			StartProcess (2);
		}

		private void StartProcess(int startArg)
		{
			switch (startArg) {
			case 0:
				ConvertWidget winConvert = new ConvertWidget (args);
				winConvert.Show ();
				break;
			case 1:
				EditWidget winEdit = new EditWidget (lastArg);
				winEdit.Show ();
				break;
			case 2:
				SteganographyWidget winSteg = new SteganographyWidget (lastArg);
				winSteg.Show ();
				break;
			}

			this.DestroyAll ();
		}
	}
}

