using System;

namespace Troonie
{
	[System.ComponentModel.ToolboxItem (true)]
	public partial class ViewerImagePanel : Gtk.Bin
	{
//		public string SurfaceFileName { 
//			get 
//			{ 
//				return simpleimagepanel.SurfaceFileName; 
//			} 
//			set 
//			{ 
//				simpleimagepanel.SurfaceFileName = value; 
//			}
//		}

		public SimpleImagePanel SimpleImagePanel { 
			get 
			{ 
				return simpleimagepanel; 
			} 
			set 
			{ 
				simpleimagepanel = value; 
			}
		}


		public ViewerImagePanel ()
		{
			this.Build ();
		}

	}
}

