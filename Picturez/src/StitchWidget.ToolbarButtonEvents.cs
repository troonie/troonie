using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public partial class StitchWidget
	{
		protected void OnToolbarBtn_OpenPressed(object sender, EventArgs e)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
				FileName01 = fc.Filename;
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
			OpenSaveAsDialog ();
		}

		protected void OnToolbarBtn_LanguagePressed (object sender, EventArgs e)
		{
			Language.I.LanguageID++;
			SetLanguageToGui ();
		}
	}
}

