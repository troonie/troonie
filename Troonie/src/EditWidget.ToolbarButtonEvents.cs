using System;
using Gtk;
using Troonie_Lib;

namespace Troonie
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
			TroonieAboutDialog.I.Run();
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

		protected int OnToolbarBtn_ShaderFilterPressed (object sender, EventArgs e, string x)
		{
			int index = filterNames.IndexOf (x);
			FilterWidget fw = null;

			switch (index) {
			case 0:
				// nothing
				break;
			case 1:
				fw = new FilterWidget (FileName, new InvertFilter());
				break;
			case 2:
				fw = new FilterWidget (FileName, new GrayscaleFilter());
				break;
			case 3:
				fw = new FilterWidget (FileName, new ExtractOrRotateChannelsFilter());
				break;
			case 4:
				fw = new FilterWidget (FileName, new GaussianBlurFilter());
				break;
			case 5:
				fw = new FilterWidget (FileName, new CannyEdgeDetectorFilter());
				break;
			case 6:
				fw = new FilterWidget (FileName, new SepiaFilter());
				break;
			case 7:
				fw = new FilterWidget (FileName, new OilPaintingFilter());
				break;
			}
//			Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
			fw.FilterEvent += FilterEvent;
			int posx, posy;
			this.GetPosition (out posx, out posy);
			fw.Move (posx + 50, posy + 50);
			fw.Show ();

			return index;
		}

		protected void OnToolbarBtn_StitchPressed (object sender, EventArgs e)
		{
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false);

			if (fc.Run() == (int)ResponseType.Ok) 
			{
//				FillImageList (new List<string>(fc.Filenames));
				StitchWidget sw = new StitchWidget(FileName, fc.Filename);

				sw.FilterEvent += FilterEvent;
				int posx, posy;
				this.GetPosition (out posx, out posy);
				sw.Move (posx + 50, posy + 50);
				sw.Show ();
			}

			fc.Destroy();
		}
	}
}

