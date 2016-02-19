using System;
using Gtk;
using Picturez_Lib;
using System.Diagnostics;

namespace Picturez
{
	[System.ComponentModel.ToolboxItem(true)]
	public partial class StarterWidget : Gtk.Window
	{
		string argString;

		public StarterWidget (string[] args) : base (Gtk.WindowType.Toplevel)
		{		
			for (int i = 0; i < args.Length; i++) {
				argString += "\"" + args [i] + "\" ";
			}

			this.Build ();
			this.KeepAbove = true;
			this.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);

			picBtnConvert.Text = Language.I.L [67];
			picBtnEdit.Text = Language.I.L [68];
			picBtnSteganography.Text = Language.I.L [80];

//			picBtnConvert.Visible = false;
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			a.RetVal = true;
		}

		protected void OnPicBtnConvertButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			StartProcess (" -c ");
		}

		private void StartProcess(string startArg)
		{
			Process p = new Process ();
			p.StartInfo.FileName = Constants.I.EXEPATH + Constants.EXENAME;
			p.StartInfo.Arguments = startArg + argString;
			p.Start ();

			Application.Quit ();
		}

		protected void OnPicBtnEditButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			StartProcess (" -e ");
		}

		protected void OnPicBtnSteganographyButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			StartProcess (" -s ");
		}
	}
}

