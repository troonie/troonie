using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public partial class EditWidget
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
			SaveAsDialog dialog = new SaveAsDialog(bt, ConvertMode.Editor);
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

		protected int OnToolbarBtn_ShaderFilterPressed (object sender, EventArgs e, string x)
		{
			int index = filterNames.IndexOf (x);

			switch (index) {
				case 0:
				Console.WriteLine ("ShaderFilter[0]: " + x);
				break;
				case 1:
				Console.WriteLine ("ShaderFilter[1]: " + x);
				break;
				default:
				Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
				break;
			}

			return index;
		}
	}
}

