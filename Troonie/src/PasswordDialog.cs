using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public delegate void PasswordDialogDelegate(string password);

	public partial class PasswordDialog : Gtk.Window
	{
		public PasswordDialogDelegate OnReleasedOkButton;

		public String OkButtontext
		{
			get { return btnOk.Text; }
			set { btnOk.Text = value; }
		}

		public PasswordDialog () : base (Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			Build ();
			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);
		}

		protected void OnBtnOkReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (entryPassword.Text.Length == 0) {
				OkCancelDialog warn = new OkCancelDialog (true);
				warn.Title = Language.I.L [118];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [119];
				warn.OkButtontext = Language.I.L [16];
				warn.Show ();

				return;
			}

			//fire the event now
			if (OnReleasedOkButton != null) //is there a EventHandler?
			{
				OnReleasedOkButton.Invoke(entryPassword.Text); //calls its EventHandler                
			} //if not, ignore
				
			this.DestroyAll ();
		}

		protected void OnEntryKeyKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			if (entryPassword.Text.Length == 0)
				return;

			char c = entryPassword.Text [entryPassword.Text.Length - 1];
			if (c == ' ') {
				entryPassword.DeleteText (entryPassword.CursorPosition - 1, entryPassword.CursorPosition);
			}

			if (args.Event.Key == Gdk.Key.Return) {
				OnBtnOkReleaseEvent (o, null);
			}
		}
	}
}

