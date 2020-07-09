using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
{
	public partial class SteganographyWidget
	{
        private const StringComparison comparison = StringComparison.CurrentCultureIgnoreCase;

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
            entrySearch.Visible = !entrySearch.Visible;
            labelSearch.Visible = !labelSearch.Visible;
            btnUp.Visible = !btnUp.Visible;
            btnDown.Visible = !btnDown.Visible;

            if (entrySearch.Visible) 
            {
                entrySearch.GrabFocus();
            }
            else 
            {
                entrySearch.Text = string.Empty;
            }
        }

        protected void OnToolbarBtn_UpArrow(object sender, EventArgs e)
        {
            int startIndex = lastCharPosOfSearch - entrySearch.Text.Length - 1;
            // avoid negative start index by searching
            if (startIndex < 0)
                return;

            TextIter ti_start, ti_end;
            int pos = textviewContent.Buffer.Text.LastIndexOf(entrySearch.Text, startIndex, comparison);

            // Set cursor to previous result
            if (pos != -1)
            {
                ti_start = textviewContent.Buffer.StartIter;
                ti_start.ForwardChars(pos);
                ti_end = textviewContent.Buffer.StartIter;
                ti_end.ForwardChars(pos + entrySearch.Text.Length);

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
            TextIter ti_start, ti_end;
            int pos = textviewContent.Buffer.Text.IndexOf(entrySearch.Text, lastCharPosOfSearch, comparison);

            // Set cursor to next result
            if (pos != -1)
            {
                ti_start = textviewContent.Buffer.StartIter;
                ti_start.ForwardChars(pos);
                ti_end = textviewContent.Buffer.StartIter;
                ti_end.ForwardChars(pos + entrySearch.Text.Length);

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

        protected void OnEntrySearch_Changed(object sender, EventArgs e)
        {
            // reset everything
            textviewContent.Buffer.RemoveTag(textTagHighlighting, textviewContent.Buffer.StartIter, textviewContent.Buffer.EndIter);
            textviewContent.Buffer.PlaceCursor(textviewContent.Buffer.StartIter);
            lastCharPosOfSearch = 0;
            countSearch = 0;
            currentNumberOfSearch = 0;

            // Allow search only for minimum char numbers
            if (entrySearch.Text.Length < 1) {
                SetSearchLabel();
                return;
            }

            TextIter ti_start, ti_end;
            int pos = textviewContent.Buffer.Text.IndexOf(entrySearch.Text, comparison);
            // Set cursor to first result
            if (pos != -1)
            {
                ti_start = textviewContent.Buffer.StartIter;
                ti_start.ForwardChars(pos);
                ti_end = textviewContent.Buffer.StartIter;
                ti_end.ForwardChars(pos + entrySearch.Text.Length);
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
            while (pos != -1)
            {
                ti_start = textviewContent.Buffer.StartIter;
                ti_start.ForwardChars(pos);
                ti_end = textviewContent.Buffer.StartIter;
                ti_end.ForwardChars(pos + entrySearch.Text.Length);

                textviewContent.Buffer.ApplyTag(textTagHighlighting, ti_start, ti_end);
                countSearch++;
                // position of next result
                pos = textviewContent.Buffer.Text.IndexOf(entrySearch.Text, pos + entrySearch.Text.Length, comparison);
            }

            SetSearchLabel();
        }
    }
}

