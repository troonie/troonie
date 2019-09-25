using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public partial class SteganographyWidget
	{
		protected void OnToolbarBtn_OpenPressed(object sender, EventArgs e)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false, false);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				FileName = fc.Filename;
				Initialize(true);
			}

			fc.Destroy();
		}			

		protected void OnToolbarBtn_SaveAsPressed (object sender, EventArgs e)
		{
			OpenSaveAsDialog ();
		}

		protected void OnToolbarBtn_LanguagePressed (object sender, EventArgs e)
		{
			Language.I.LanguageID++;
			SetLanguageToGui ();
		}


        protected void OnToolbarBtn_Search(object sender, EventArgs e)
        {
            searchEntry.Visible = !searchEntry.Visible;
            searchLabel.Visible = !searchLabel.Visible;
            up_button.Visible = !up_button.Visible;
            down_button.Visible = !down_button.Visible;
        }

        protected void OnToolbarBtn_UpArrow(object sender, EventArgs e)
        {
         
        }

        protected void OnToolbarBtn_DownArrow(object sender, EventArgs e)
        {

        }

        protected void OnSearchEntry_Changed(object sender, EventArgs e)
        {
            //Console.WriteLine("Changed: " + searchEntry.Text);
            TextIter ti_start, ti_end;
            // reset everything
            textviewContent.Buffer.RemoveTag(tt_Highlight, textviewContent.Buffer.StartIter, textviewContent.Buffer.EndIter);
            textviewContent.Buffer.PlaceCursor(textviewContent.Buffer.StartIter);
            lastCharPosOfSearch = 0;
            countSearch = 0;
            currentNumberOfSearch = 0;

            // Allow search only for minimum char numbers
            if (searchEntry.Text.Length < 1) {
                SetSearchLabel();
                return;
            }

            ti_temp = textviewContent.Buffer.StartIter;
            // Set cursor to first result
            if (ti_temp.ForwardSearch(searchEntry.Text, TextSearchFlags.VisibleOnly, out ti_start, out ti_end, textviewContent.Buffer.EndIter))
            {
                textviewContent.Buffer.PlaceCursor(ti_start);
                textviewContent.Buffer.SelectRange(ti_start, ti_end);
                currentNumberOfSearch++;
                lastCharPosOfSearch = ti_end.Offset;

                if (scrolledwindowContent.VScrollbar.Visible)
                {
                    scrolledwindowContent.Vadjustment.Value = scrolledwindowContent.Vadjustment.Upper * ti_start.Line / textviewContent.Buffer.LineCount;
                }
            }

            // Highlight all results
            while (ti_temp.ForwardSearch(searchEntry.Text, TextSearchFlags.VisibleOnly, out ti_start, out ti_end, textviewContent.Buffer.EndIter))
            {
                textviewContent.Buffer.ApplyTag(tt_Highlight, ti_start, ti_end);
                ti_temp = textviewContent.Buffer.StartIter;
                ti_temp.ForwardChars(ti_end.Offset);
                countSearch++;
            }

            SetSearchLabel();
        }

        //[GLib.ConnectBefore()]
        //protected void OnsearchEntryKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        //{
        //    string search = searchEntry.Text;
        //    TextIter ti_start, ti_end;
        //    // reset everything
        //    textviewContent.Buffer.RemoveTag(tt_Highlight, textviewContent.Buffer.StartIter, textviewContent.Buffer.EndIter);
        //    textviewContent.Buffer.PlaceCursor(textviewContent.Buffer.StartIter);

        //    // Allow search only for minimum char numbers
        //    //if (search.Length < 2) {
        //    //    args.RetVal = true;
        //    //    return;
        //    //}

        //    ti_temp = textviewContent.Buffer.StartIter;
        //    // Set cursor to first result
        //    if (ti_temp.ForwardSearch(search, TextSearchFlags.VisibleOnly, out ti_start, out ti_end, textviewContent.Buffer.EndIter)) 
        //    {
        //        textviewContent.Buffer.PlaceCursor(ti_start);
        //        textviewContent.Buffer.SelectRange(ti_start, ti_end);
        //        if (scrolledwindowContent.VScrollbar.Visible)
        //        {
        //            scrolledwindowContent.Vadjustment.Value = scrolledwindowContent.Vadjustment.Upper * ti_start.Line / textviewContent.Buffer.LineCount;
        //        }
        //    }

        //    // Highlight all results
        //    while (ti_temp.ForwardSearch(search, TextSearchFlags.VisibleOnly, out ti_start, out ti_end, textviewContent.Buffer.EndIter))
        //    {
        //        textviewContent.Buffer.ApplyTag(tt_Highlight, ti_start, ti_end);
        //        ti_temp = textviewContent.Buffer.StartIter;
        //        ti_temp.ForwardChars(ti_end.Offset);
        //    }

        //    args.RetVal = true;
        //}
    }
}

