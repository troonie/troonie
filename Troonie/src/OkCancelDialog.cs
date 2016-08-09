using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public delegate void OnReleasedButtonDelegate();

	public partial class OkCancelDialog : Gtk.Window
	{
		public OnReleasedButtonDelegate OnReleasedOkButton;
		public OnReleasedButtonDelegate OnReleasedCancelButton;

		public String OkButtontext
		{
			get { return picBtnOk.Text; }
			set { picBtnOk.Text = value; }
		}

		public String CancelButtontext
		{
			get { return picBtnCancel.Text; }
			set { picBtnCancel.Text = value; }
		}

		public String Label1
		{
			get { return label1.Text; }
			set { label1.LabelProp = "<b>" + value + "</b>"; }
		}

		public String Label2
		{
			get { return label2.Text; }
			set { label2.Text = value; }
		}

		public OkCancelDialog (bool showOnlyOkButton) : 
				base(Gtk.WindowType.Toplevel)
		{
			KeepAbove = true;
			Build ();
			ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);	

			if (showOnlyOkButton)
				picBtnCancel.Visible = false;
		}

		protected void OnPicBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//fire the event now
			if (OnReleasedOkButton != null) //is there a EventHandler?
			{
				OnReleasedOkButton.Invoke(); //calls its EventHandler                
			} //if not, ignore

//			Destroy();
			this.DestroyAll ();
		}

		protected void OnPicBtnCancelButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			//fire the event now
			if (OnReleasedCancelButton != null) //is there a EventHandler?
			{
				OnReleasedCancelButton.Invoke(); //calls its EventHandler                
			} //if not, ignore

//			Destroy();
			this.DestroyAll ();
		}

		[GLib.ConnectBefore ()] 
		protected void OnKeyPressEvent (object o, KeyPressEventArgs args)
		{
			//			System.Console.WriteLine("Keypress: {0}  -->  State: {1}", args.Event.Key, args.Event.State); 

			if (args.Event.Key == Gdk.Key.Return && !picBtnCancel.Visible) {
				OnPicBtnOkButtonReleaseEvent(o, null);
			}
		}
	}
}

