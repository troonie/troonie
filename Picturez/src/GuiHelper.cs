using System;
using Gtk;
using Picturez_Lib;

namespace Picturez
{
	public delegate void OnToolbarBtnPressed (object sender, EventArgs e);
	public delegate int OnToolbarMenuBarAsBtnPressed (object sender, EventArgs e, string x);

	public class GuiHelper
	{
		private static GuiHelper instance;
		public static GuiHelper I
		{
			get
			{
				if (instance == null)
					instance = new GuiHelper ();

				return instance;
			}
		}

		public void SetPanelSize(Window window, SimpleImagePanel simpleimagepanel, HBox hbox, int maxPanelWidth, int maxPanelHeight, int imageW, int imageH, int minWinWidth = 0, int minWinHeight = 0)
		{		
			const int optionsWidth = 390;
			// general taskbar size in win_8.1
			const int taskbarHeight = 90;
			const int paddingOffset = 44;
			// necessary to correct to small height 
			const float multiplicatorHeight = 1.2f;

			//			Gdk.Screen screen = this.Screen;
			//			int monitor = screen.GetMonitorAtWindow (this.GdkWindow); 
			//			Gdk.Rectangle bounds = screen.GetMonitorGeometry (monitor);
			//			int winW = bounds.Width;
			// DIFFERENCE 1 to EditWidget
			//			int winH = bounds.Height - taskbarHeight - 300;
			int winW;
			int winH;

			// DIFFERENCE 2 to EditWidget
			//			int panelW = winW - optionsWidth - paddingOffset;
			//			int panelH = winH - (int)(paddingOffset * multiplicatorHeight);
//			int panelW = 400;
//			int panelH = 300;

			// setting padding for left and right side
			Box.BoxChild w4 = ((Box.BoxChild)(hbox [simpleimagepanel]));
			w4.Padding = ((uint)(paddingOffset / 4.0f + 0.5f));

			if (maxPanelWidth < imageW || maxPanelHeight < imageH)
			{
				bool wLonger = (imageW / (float)imageH) > (maxPanelWidth / (float)maxPanelHeight);
				if (wLonger)
				{
					maxPanelHeight = (int)(imageH * maxPanelWidth / (float)imageW  + 0.5f);
					//					winH = panelH + (int)(paddingOffset * multiplicatorHeight);
					//					winW = panelW + optionsWidth + paddingOffset;
				}
				else
				{
					maxPanelWidth = (int)(imageW * maxPanelHeight / (float)imageH  + 0.5f);
					//					winW = panelW + optionsWidth + paddingOffset;
					//					winH = panelH + (int)(paddingOffset * multiplicatorHeight);
				}
			}
			else
			{
				maxPanelWidth = imageW;
				maxPanelHeight = imageH;
				//				winW = panelW + optionsWidth + paddingOffset;
				//				winH = panelH + (int)(paddingOffset * multiplicatorHeight);
			}	

			winW = Math.Max(minWinWidth, maxPanelWidth + optionsWidth + paddingOffset);
			winH = Math.Max(minWinHeight, maxPanelHeight + (int)(paddingOffset * multiplicatorHeight));

			simpleimagepanel.WidthRequest = maxPanelWidth;
			simpleimagepanel.HeightRequest = maxPanelHeight;

			simpleimagepanel.ScaleCursorX = imageW / (float)maxPanelWidth;
			simpleimagepanel.ScaleCursorY = imageH / (float)maxPanelHeight;

			window.WidthRequest = winW;
			window.HeightRequest = winH;


			// work around by GLib timeout to fix the GTK#-resizing bug
			GLib.TimeoutHandler timeoutHandler = () => {
				window.WidthRequest = winW;
				window.HeightRequest = winH;
				window.Move (0, 0);
				window.Resize (winW, winH);
				// false, because usage only one time
				return false;
			};
			GLib.Timeout.Add(200, timeoutHandler);
		}

		public FileChooserDialog GetImageFileChooserDialog(bool selectMultiple)
		{
			object[] o = new object[]{Language.I.L[16],ResponseType.Ok, 
				Language.I.L[17],ResponseType.Cancel};
			string title = selectMultiple ? Language.I.L[39] : Language.I.L[2];

			FileChooserDialog fc =
				new FileChooserDialog(title,
					null,
					FileChooserAction.Open,
					o);

			fc.SelectMultiple = selectMultiple;
			FileFilter filter  = new FileFilter();
			filter.Name = "Image files";
			// filter.AddMimeType ("image/png");
			filter.AddPattern("*.jpg");
			filter.AddPattern("*.jpeg");
			filter.AddPattern("*.png");
			filter.AddPattern("*.tif");
			filter.AddPattern("*.bmp");
			filter.AddPattern("*.gif");
			filter.AddPattern("*.tiff");
			filter.AddPattern("*.JPG");
			filter.AddPattern("*.JPEG");
			filter.AddPattern("*.PNG");
			filter.AddPattern("*.TIF");
			filter.AddPattern("*.BMP");
			filter.AddPattern("*.GIF");
			filter.AddPattern("*.TIFF");
			fc.Filter = filter;
			// fc.RemoveShortcutFolderUri (Environment.GetFolderPath(Environment.SpecialFolder.Recent));

			return fc;
		}

		public void CreateMenubarInToolbar(HBox hboxToolbarButtons, int position, string stockicon, 
		                                   OnToolbarMenuBarAsBtnPressed pressed, string[] menuitems)
		{
			MenuBar mb = new MenuBar();
			mb.ModifyBg(StateType.Normal, ColorConverter.Instance.GRID);

			Menu filemenu = new Menu();
			// MenuItem file = new MenuItem("File");
			ImageMenuItem rootitem = new ImageMenuItem(menuitems[0]);
			rootitem.Image = Image.LoadFromResource(stockicon);
			rootitem.Image.Visible = true;
			rootitem.Submenu = filemenu;


			for (int i = 1; i < menuitems.Length; i++) {
				MenuItem item = new MenuItem(menuitems[i]);
				AccelLabel al = item.Child as AccelLabel;
				item.Activated += (sender, e) => pressed(sender, e, al.Text);
				filemenu.Append(item);
			}

			mb.Append(rootitem);
			hboxToolbarButtons.Add (mb);

			Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons [mb];
			w3x.Position = position;
			w3x.Expand = false;
			w3x.Fill = false;
		}

		public void CreateToolbarIconButton(HBox hboxToolbarButtons, int position, string stockicon, OnToolbarBtnPressed pressed, string label = null)
		{
			Button l_button = new Button();
			l_button.Image = Image.LoadFromResource(stockicon);
			l_button.Visible = true;
			l_button.Label = label;
			l_button.Image.Visible = true;
			l_button.Pressed += new EventHandler (pressed);
			hboxToolbarButtons.Add (l_button);		
			Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons [l_button];
			w3x.Position = position;
			w3x.Expand = false;
			w3x.Fill = false;
		}

		public void CreateToolbarSeparator(HBox hboxToolbarButtons, int position)
		{
			VSeparator vsep = new VSeparator ();
			vsep.Visible = true;
			hboxToolbarButtons.Add (vsep);		
			Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons [vsep];
			w3x.Position = position;
			w3x.Expand = false;
			w3x.Fill = false;
		}
	}
}

