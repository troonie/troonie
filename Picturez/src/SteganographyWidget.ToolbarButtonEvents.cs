using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public partial class SteganographyWidget
	{
		protected void OnToolbarBtn_OpenPressed(object sender, EventArgs e)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				FileName = fc.Filename;
				Initialize(true);
			}

			fc.Destroy();
		} 

		protected virtual void OnToolbarBtn_AboutPressed(object sender, EventArgs e)
		{
			PicturezAboutDialog.I.Run();
		} 

		protected void OnToolbarBtn_SaveAsPressed (object sender, EventArgs e)
		{
			SaveAsDialog dialog = new SaveAsDialog(bt);
			dialog.AllowOnlyPng32BitAlphaAsValueSaving ();
			if (dialog.Run () == (int)ResponseType.Ok) {
				if (dialog.Process ()) {
					FileName = dialog.SavedFileName;
					bt.Dispose ();
					Initialize(true);
				}				
			}

			dialog.Destroy();
		}

		protected void OnToolbarBtn_LanguagePressed (object sender, EventArgs e)
		{
			Language.I.LanguageID++;
			SetLanguageToGui ();
		}
	}
}

