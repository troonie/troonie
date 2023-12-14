using System;
using Gtk;

namespace Troonie
{
	public partial class EditWidget
	{
		#region Entry events

		private static void DoKeyReleaseEvent(Entry en, ImagePanel ip, Slider s, int widthOrHeight)
		{
			en.ModifyBg(StateType.Normal, ColorConverter.Instance.White);

			int number = 0;
			bool isNumber = int.TryParse (en.Text, out number);
			if (!isNumber && en.Text.Length != 0) {
				en.DeleteText (en.CursorPosition - 1, en.CursorPosition);
				DoKeyReleaseEvent (en, ip, s, widthOrHeight);
				return;
			}

			if (number <= widthOrHeight && ip.MoveSliderByValue (s, number)) {
				ip.QueueDraw ();
			} else {
				en.ModifyBg(StateType.Normal, ColorConverter.Instance.Red);
			}
		}

		protected void OnEntryLeftKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			DoKeyReleaseEvent (entryLeft, imagepanel1, imagepanel1.LeftSlider, bt.Bitmap.Width);
		}

		protected void OnEntryRightKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			DoKeyReleaseEvent (entryRight, imagepanel1, imagepanel1.RightSlider, bt.Bitmap.Width);
		}

		protected void OnEntryTopKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			DoKeyReleaseEvent (entryTop, imagepanel1, imagepanel1.TopSlider, bt.Bitmap.Height);
		}

		protected void OnEntryBottomKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			DoKeyReleaseEvent (entryBottom, imagepanel1, imagepanel1.BottomSlider, bt.Bitmap.Height);
		}

		protected void OnEntryRotateKeyReleaseEvent (object o, KeyReleaseEventArgs args)
		{
			int number = 0;
			if (entryRotate.Text.Length == 0 || int.TryParse (entryRotate.Text, out number)) {
				imagepanel1.Angle = number / 10.0;
				imagepanel1.QueueDraw ();
			} else {
				entryRotate.DeleteText (entryRotate.CursorPosition - 1, entryRotate.CursorPosition);
			}
		}
		#endregion Entry events
	}
}

