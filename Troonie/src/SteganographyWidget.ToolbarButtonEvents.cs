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

            if (searchEntry.Visible) 
            {
                searchEntry.GrabFocus();
            }
            else 
            {
                searchEntry.Text = string.Empty;
            }
        }

        protected void OnToolbarBtn_UpArrow(object sender, EventArgs e)
        {
            ti_temp = textviewContent.Buffer.StartIter;
            ti_temp.ForwardChars(lastCharPosOfSearch);

            ti_temp.BackwardChars(searchEntry.Text.Length);
            // Set cursor to previous result
            if (ti_temp.BackwardSearch(searchEntry.Text, TextSearchFlags.VisibleOnly, out TextIter ti_start, out TextIter ti_end, textviewContent.Buffer.StartIter))
            {
                textviewContent.Buffer.PlaceCursor(ti_start);
                textviewContent.Buffer.SelectRange(ti_start, ti_end);
                currentNumberOfSearch--;
                SetSearchLabel();
                lastCharPosOfSearch = ti_end.Offset;

                if (scrolledwindowContent.VScrollbar.Visible)
                {
                    scrolledwindowContent.Vadjustment.Value = scrolledwindowContent.Vadjustment.Upper * ti_start.Line / textviewContent.Buffer.LineCount;
                }
            }
        }

        protected void OnToolbarBtn_DownArrow(object sender, EventArgs e)
        {
            ti_temp = textviewContent.Buffer.StartIter;
            ti_temp.ForwardChars(lastCharPosOfSearch);

            // Set cursor to next result
            if (ti_temp.ForwardSearch(searchEntry.Text, TextSearchFlags.VisibleOnly, out TextIter ti_start, out TextIter ti_end, textviewContent.Buffer.EndIter))
            {
                textviewContent.Buffer.PlaceCursor(ti_start);
                textviewContent.Buffer.SelectRange(ti_start, ti_end);
                currentNumberOfSearch++;
                SetSearchLabel();
                lastCharPosOfSearch = ti_end.Offset;

                if (scrolledwindowContent.VScrollbar.Visible)
                {
                    scrolledwindowContent.Vadjustment.Value = scrolledwindowContent.Vadjustment.Upper * ti_start.Line / textviewContent.Buffer.LineCount;
                }
            }
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
    }
}

