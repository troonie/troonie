using System;
using Gtk;
using System.Drawing;
using Troonie_Lib;
using ImageConverter = Troonie_Lib.ImageConverter;

namespace Troonie
{
	public partial class EditWidget
	{
		#region button events

		protected void OnBtnOkReleased (object sender, EventArgs e)
		{
			Bitmap b1;

			config.Left = int.Parse (entryLeft.Text);
			config.Right = int.Parse (entryRight.Text);
			config.Top =  int.Parse (entryTop.Text);
			config.Bottom =  int.Parse (entryBottom.Text);
			config.Rotation =  int.Parse (entryRotate.Text);

			ConfigEdit.Save (config);

			if (imagepanel1.Angle != 0) {
				Console.WriteLine ("imagepanel1.Angle != 0, aber: =" + imagepanel1.Angle );
				RotateBilinear rb = new RotateBilinear (imagepanel1.Angle, false);
				b1 = imagepanel1.Angle == 0 ? bt.Bitmap : rb.Apply (bt.Bitmap);
			} else {
				int w = (int)Math.Round (imagepanel1.LeftSlider.DistXToPartner * imagepanel1.ScaleCursorX);
				int h = (int)Math.Round (imagepanel1.TopSlider.DistYToPartner * imagepanel1.ScaleCursorY);
				float xStart = (float)Math.Round (imagepanel1.LeftSlider.XGlobal * imagepanel1.ScaleCursorX);
				float yStart = (float)Math.Round (imagepanel1.TopSlider.YGlobal * imagepanel1.ScaleCursorY);

				ImageConverter.ScaleAndCut (
					bt.Bitmap, 
					out b1,
					xStart, 
					yStart,
					w,
					h,
					ConvertMode.Editor,
					true);
			}

			bt.Bitmap.Dispose ();
			bt.ChangeBitmapButNotTags(b1);

			Initialize (false);
		}	

		protected void OnButtonReleaseEvent (object o, ButtonReleaseEventArgs args)
		{
			timeoutSw.Stop ();
			repeatTimeout = false;
		}

		protected void OnBtnRotatePlusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutRotateValue = 1;
			repeatTimeout = true;
			Rotate ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(RotateByTimeoutHandler));
		}

		protected void OnBtnRotateMinusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutRotateValue = -1;
			repeatTimeout = true;
			Rotate ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(RotateByTimeoutHandler));
		}

		protected void OnBtnLeftPlusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.LeftSlider;
			timeoutKey = Gdk.Key.Right;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnLeftMinusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.LeftSlider;
			timeoutKey = Gdk.Key.Left;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnBottomMinusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.BottomSlider;
			timeoutKey = Gdk.Key.Down;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}
		protected void OnBtnBottomPlusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.BottomSlider;
			timeoutKey = Gdk.Key.Up;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnTopMinusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.TopSlider;
			timeoutKey = Gdk.Key.Up;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnTopPlusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.TopSlider;
			timeoutKey = Gdk.Key.Down;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnRightMinusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.RightSlider;
			timeoutKey = Gdk.Key.Right;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		protected void OnBtnRightPlusButtonPressEvent (object o, ButtonPressEventArgs args)
		{
			if (repeatTimeout)
				return;

			timeoutSw.Restart ();
			timeoutSlider = imagepanel1.RightSlider;
			timeoutKey = Gdk.Key.Left;
			repeatTimeout = true;
			MoveTimeoutSlider ();
			GLib.Timeout.Add(Constants.TIMEOUT_INTERVAL, new GLib.TimeoutHandler(MoveSliderByTimeoutHandler));
		}

		#endregion button events	
	}
}

