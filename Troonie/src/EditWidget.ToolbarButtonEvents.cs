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

		protected void OnToolbarBtn_ShaderFilterPressed (object sender, EventArgs e, string x)
		{
			FilterWidget fw = null;

			if (x == filterN.OVERVIEW) {
				// just overview button name, do nothing
			} else if (x == filterN.Invert) {
				fw = new FilterWidget (FileName, new InvertFilter ());
			} else if (x == filterN.Grayscale) {
				fw = new FilterWidget (FileName, new GrayscaleFilter());
			} else if (x == filterN.RGB_Channels) {
				fw = new FilterWidget (FileName, new ExtractOrRotateChannelsFilter());
			} else if (x == filterN.Gaussian_blur) {
				fw = new FilterWidget (FileName, new GaussianBlurFilter(), false);
			}
			else if (x == filterN.Canny_edge_detector) {
				fw = new FilterWidget (FileName, new CannyEdgeDetectorFilter());
			}
			else if (x == filterN.Sepia) {
				fw = new FilterWidget (FileName, new SepiaFilter());
			}
			else if (x == filterN.Oil_painting) {
				fw = new FilterWidget (FileName, new OilPaintingFilter());
			}
			else if (x == filterN.Difference) {
				#region filterN.Difference
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
						return; 
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
						return; 
					}

					DifferenceFilter diff = new DifferenceFilter ();
					diff.ImagesPaths = new[] {FileName, fc.Filename};
					fw = new FilterWidget (FileName, diff);
					fc.Destroy ();
					// break;
				} else {
					fc.Destroy ();
					return; 
				}
				#endregion filterN.Difference
			}
			else if (x == filterN.Blend) {
				#region filterN.Blend
				FileChooserDialog fc = GuiHelper.I.GetImageFileChooserDialog (true, false, Language.I.L [306]);
				if (fc.Run () == (int)ResponseType.Ok) {
					BlendFilter blend;
					switch (fc.Filenames.Length) {
					case 1:
						blend = new BlendFilter ();
						break;
					case 2:
						blend = new BlendFilter3Images ();
						break;
					case 3:
					default:
						blend = new BlendFilter4Images ();
						break;
					}

					int max3 = fc.Filenames.Length > 3 ? 3 : fc.Filenames.Length; 
					blend.ImagesPaths = new string[max3 + 1];
					blend.ImagesPaths[0] = FileName;

					for (int i = 0; i < max3; i++) {
						blend.ImagesPaths[i + 1] = fc.Filenames[i]; 	
					}
//					blend.ImagesPaths = new[] {FileName, fc.Filename};
					fw = new FilterWidget (FileName, blend);
					fc.Destroy ();
					// break;
				} else {
					fc.Destroy ();
					return; 
				}
				#endregion filterN.Blend
			}
			else if (x == filterN.Posterization) {
				fw = new FilterWidget (FileName, new PosterizationFilter());
			}
			else if (x == filterN.Cartoon) {
				fw = new FilterWidget (FileName, new CartoonFilter());
			}
			else if (x == filterN.Sobel_edge_detector) {
				fw = new FilterWidget (FileName, new SobelEdgeDetectorFilter());
			}
			else if (x == filterN.Unsharp_masking) {
				fw = new FilterWidget (FileName, new GaussianBlurFilter(), true);
			}
			else if (x == filterN.Sobel_edge_marker) {
				fw = new FilterWidget (FileName, new SobelEdgeMarkerFilter());
			}
			else if (x == filterN.Binarization) {
				fw = new FilterWidget (FileName, new BinarizationFilter());
			}
			else if (x == filterN.Meanshift) {
				MessageDialog md = new MessageDialog (this, 
					                   DialogFlags.DestroyWithParent, MessageType.Question, 
					ButtonsType.OkCancel, Language.I.L [291]);
				ResponseType response = (ResponseType)md.Run ();
				md.Destroy ();
				if (response == ResponseType.Cancel) {
					return;
				}
				fw = new FilterWidget (FileName, new MeanshiftFilter ());
			}
			else if (x == filterN.Edge_Point_dilatation) {
				fw = new FilterWidget (FileName, new DilatationFilter());
			}
			else if (x == filterN.Exponentiate_channels) {
				fw = new FilterWidget (FileName, new ExponentiateChannelsFilter());
			}
			else if (x == filterN.Convolution5x5) {
				fw = new FilterWidget (FileName, new Convolution5X5Filter());
			}

//			Console.WriteLine ("ShaderFilter[" + index + "]: " + x);
			fw.FilterEvent += FilterEvent;
			int posx, posy;
			this.GetPosition (out posx, out posy);
			fw.Move (posx + 50, posy + 50);
			fw.Show ();

			return;
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

