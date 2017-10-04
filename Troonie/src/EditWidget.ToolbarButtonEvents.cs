using System;
using Gtk;
using Troonie_Lib;
using System.Drawing.Imaging;
using IOPath = System.IO.Path;

namespace Troonie
{
	public partial class EditWidget
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
				fw = new FilterWidget (FileName, new GaussianBlurFilter(), false);
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
			case 8:
				FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false, false, Language.I.L [152]);
				if (fc.Run () == (int)ResponseType.Ok) {
					int w, h, wCompare, hCompare, psCompare;
					PixelFormat pf;
					w = bt.Bitmap.Width;
					h = bt.Bitmap.Height;
					pf = bt.Bitmap.PixelFormat; 
					// GetImageDimension(..) does not work under Windows
					//	ImageConverter.GetImageDimension (FileName, out w, out h, out pf);
					int ps = System.Drawing.Image.GetPixelFormatSize(pf) / 8;
					if (FileName.Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar) == 
						fc.Filename.Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar)) {
						wCompare = w;
						hCompare = h;
						psCompare = ps;
					} else {
						ImageConverter.GetImageDimension (fc.Filename, out wCompare, out hCompare, out pf);
						psCompare = System.Drawing.Image.GetPixelFormatSize(pf) / 8;
					}

					if (Math.Abs(psCompare - ps) > 1) {
//						string errorMsg = "Cannot compare grayscale with color image.";
//						throw new ArgumentException(errorMsg);
						OkCancelDialog warn = new OkCancelDialog (true);
						warn.Title = Language.I.L [153];
						warn.Label1 = Language.I.L [154];
						warn.Label2 = string.Empty;
						warn.OkButtontext = Language.I.L [16];
						warn.Show ();
						fc.Destroy ();
						return index; 
					}

					if (w != wCompare || h != hCompare) {
//						string errorMsg = "Cannot compare different image sizes.";
//						throw new ArgumentException(errorMsg);
						OkCancelDialog warn = new OkCancelDialog (true);
						warn.Title = Language.I.L [153];
						warn.Label1 = Language.I.L [155];
						warn.Label2 = string.Empty;
						warn.OkButtontext = Language.I.L [16];
						warn.Show ();
						fc.Destroy ();
						return index; 
					}

					DifferenceFilter diff = new DifferenceFilter ();
					diff.CompareBitmap = new System.Drawing.Bitmap (fc.Filename);
					fw = new FilterWidget (FileName, diff);
					fc.Destroy ();
					break;
				} else {
					fc.Destroy ();
					return index; 
				}
			case 9:
				fw = new FilterWidget (FileName, new PosterizationFilter());
				break;
			case 10:
				fw = new FilterWidget (FileName, new CartoonFilter());
				break;
			case 11:
				fw = new FilterWidget (FileName, new SobelEdgeDetectorFilter());
				break;
			case 12:
				fw = new FilterWidget (FileName, new GaussianBlurFilter (), true);
				break;
			case 13:
				fw = new FilterWidget (FileName, new SobelEdgeMarkerFilter ());
				break;
			case 14:
				fw = new FilterWidget (FileName, new BinarizationFilter ());
				break;
			case 15:
				MessageDialog md = new MessageDialog (this, 
					                   DialogFlags.DestroyWithParent, MessageType.Question, 
					ButtonsType.OkCancel, Language.I.L [291]);
				ResponseType response = (ResponseType)md.Run ();
				md.Destroy ();
				if (response == ResponseType.Cancel) {
					return index;
				}
				fw = new FilterWidget (FileName, new MeanshiftFilter ());
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
			FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (false, false, Language.I.L[151]);

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

