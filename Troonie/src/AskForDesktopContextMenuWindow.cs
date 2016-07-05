using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public partial class AskForDesktopContextMenuWindow : Gtk.Window
	{
		private bool isProgramstart;
		private Config current;

		public AskForDesktopContextMenuWindow (bool isProgramstart, Config current) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.current = current;
			this.isProgramstart = isProgramstart;
			this.KeepAbove = true;
			Build ();
			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);

			Title = Language.I.L[59];
			label1.LabelProp = "<b>" + Language.I.L[57] + "</b>";
			label2.Text = Language.I.L[58];
			chkBtn.Label = Language.I.L[60];

			if (isProgramstart) {
				picbtnYes.Text = Language.I.L [61];
				picbtnNo.Text = Language.I.L [62];
			} else {
				chkBtn.Visible = false;
				picbtnYes.Text = Language.I.L [64];
				picbtnNo.Text = Language.I.L [65];
			}
		}

		protected void OnPicbtnYesButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (Constants.I.WINDOWS)
				DesktopContextMenu.I.WindowsDesktopContextMenu(true);
			else 
				DesktopContextMenu.I.LinuxDesktopContextMenu (true);			

			current.AskForDesktopContextMenu = false;

			MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, Language.I.L[63]);
			md.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
			md.KeepAbove = true;
			if (md.Run () == (int)ResponseType.Ok) {
				md.Destroy ();
				this.Destroy ();
			}
		}	

		protected void OnPicbtnNoButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (!isProgramstart) {
				if (Constants.I.WINDOWS)
					DesktopContextMenu.I.WindowsDesktopContextMenu(false);
				else 
					DesktopContextMenu.I.LinuxDesktopContextMenu(false);
				
				current.AskForDesktopContextMenu = true;

				MessageDialog md = new MessageDialog(this, DialogFlags.DestroyWithParent, MessageType.Info, ButtonsType.Ok, Language.I.L[66]);
				md.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
				md.KeepAbove = true;
				if (md.Run () == (int)ResponseType.Ok) {
					md.Destroy ();
				}				
			}

			this.Destroy ();
		}

		protected void OnChkBtnToggled (object sender, EventArgs e)
		{
			current.AskForDesktopContextMenu = chkBtn.Active;
		}
	}
}

