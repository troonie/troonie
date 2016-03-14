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
			bool runDialog = true;

			do
			{
				if (dialog.Run () == (int)ResponseType.Ok) {
					if (dialog.Process ()) {
						FileName = dialog.SavedFileName;
						bt.Dispose ();
						Initialize (true);
						runDialog = false;
					}
				}
				else {
					runDialog = false;
				}
			}
			while (runDialog);

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
			FilterWidget fw = null;

			switch (index) {
			case 0:
				Console.WriteLine ("ShaderFilter[0]: " + x);
				break;
			case 1:
				Console.WriteLine ("ShaderFilter[1]: " + x);
				fw = new FilterWidget (FileName, new InvertFilter());
				break;
			case 2:
				Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
				fw = new FilterWidget (FileName, new GrayscaleFilter());
				break;
			case 3:
				Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
				fw = new FilterWidget (FileName, new ExtractOrRotateChannelsFilter());
				break;
			case 4:
				Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
				fw = new FilterWidget (FileName, new GaussianBlurFilter());
				break;
			case 5:
				Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
				fw = new FilterWidget (FileName, new CannyEdgeDetectorFilter());
				break;
			}

			fw.FilterEvent += FilterEvent;
			int posx, posy;
			this.GetPosition (out posx, out posy);
			fw.Move (posx + 50, posy + 50);
			fw.Show ();

			return index;
		}
	}
}

