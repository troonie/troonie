using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public partial class StitchWidget
	{
		protected void OnBtn01ImageChoosePressEvent (object o, ButtonPressEventArgs args)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				Console.WriteLine (fc.Filename);
			}

			fc.Destroy();
		}

		protected void OnBtnOkButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			if (entryKey.Text.Length == 0) {
				PseudoPicturezContextMenu warn = new PseudoPicturezContextMenu (true);
				warn.Title = Language.I.L [118];
				warn.Label1 = string.Empty;
				warn.Label2 = Language.I.L [119];
				warn.OkButtontext = Language.I.L [16];
				//				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				return;
			}

			if (rdBtnPortrait.Active && entryKey.Text.Length < 10) {
				PseudoPicturezContextMenu warn = new PseudoPicturezContextMenu (false);
				warn.Title = Language.I.L [109];
				warn.Label1 = Language.I.L [110] + entryKey.Text.Length + Language.I.L [111];
				warn.Label2 = Language.I.L [112];
				warn.OkButtontext = Language.I.L [16];
				warn.CancelButtontext = Language.I.L [17];	
				warn.Show ();

				warn.OnReleasedOkButton += DoStitch;
			} else {
				DoStitch ();
			}
		}

		protected void OnBtnReleaseEvent (object o, ButtonReleaseEventArgs args)
		{

			timeoutSw.Stop ();
			repeatTimeout = false;
		}

		protected void OnBtnPressEvent (object o, ButtonPressEventArgs args)
		{
			if (o == btn01LeftPlus) {
				pointerLabel = lb01Left;
				incrementValue = true;
			}
			else if (o == btn01LeftMinus) {
				pointerLabel = lb01Left;
				incrementValue = false;
			}
			else if (o == btn01RightPlus) {
				pointerLabel = lb01Right;
				incrementValue = true;
			}
			else if (o == btn01RightMinus) {
				pointerLabel = lb01Right;
				incrementValue = false;
			}
			else if (o == btn01TopPlus) {
				pointerLabel = lb01Top;
				incrementValue = true;
			}
			else if (o == btn01TopMinus) {
				pointerLabel = lb01Top;
				incrementValue = false;
			}
			else if (o == btn01BottomPlus) {
				pointerLabel = lb01Bottom;
				incrementValue = true;
			}
			else if (o == btn01BottomMinus) {
				pointerLabel = lb01Bottom;
				incrementValue = false;
			}

			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			repeatTimeout = true;
			SetImagePaddingAndLabel ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(SetImagePaddingAndLabelByTimeoutHandler));
		}

		private void SetImagePaddingAndLabel()
		{
			int v = int.Parse (pointerLabel.Text);
			if (incrementValue && v < maxpadding) {
				v++;
			}
			else if (!incrementValue && v > 0) {
				v--;
			}
			pointerLabel.Text = v.ToString ();
		}

		private bool SetImagePaddingAndLabelByTimeoutHandler()
		{
			if (timeoutSw.ElapsedMilliseconds < Constants.TIMEOUT_INTERVAL_FIRST) {
				return repeatTimeout;
			}

			SetImagePaddingAndLabel();
			return repeatTimeout;
		}
	}
}

