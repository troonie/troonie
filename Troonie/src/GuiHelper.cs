using System;
using Gtk;
using Troonie_Lib;
using System.Globalization;

namespace Troonie
{
	public delegate void OnToolbarBtnPressed (object sender, EventArgs e);
	public delegate void OnToolbarMenuBarAsBtnPressed (object sender, EventArgs e, string x);

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

		private float newVersion;

		private void UpdatePressed (object sender, EventArgs e)
		{
			OkCancelDialog pseudo = new OkCancelDialog (false);
			pseudo.Title = Language.I.L [69];
			pseudo.Label1 = Language.I.L [70] + newVersion.ToString(CultureInfo.InvariantCulture);
			pseudo.Label2 = Language.I.L [71];
			pseudo.OkButtontext = Language.I.L [16];
			pseudo.CancelButtontext = Language.I.L [17];
			// pseudo.OnReleasedOkButton += delegate{Process.Start (Constants.WEBSITE);};
			pseudo.OnReleasedOkButton += () => System.Diagnostics.Process.Start (Constants.WEBSITE);

			pseudo.Show ();
		}

		private void CreateToolbarUpdateButton(HBox hboxToolbarButtons, int position, string stockicon, string tooltipText, float newVersion)
		{
			this.newVersion = newVersion;
			Button l_button = new Button();
			l_button.Image = Image.LoadFromResource(stockicon);
			l_button.Visible = true;
			l_button.TooltipText = tooltipText + newVersion.ToString(CultureInfo.InvariantCulture);
			l_button.Label = Language.I.L[69];
			l_button.Image.Visible = true;
			l_button.Pressed += UpdatePressed;
			hboxToolbarButtons.Add (l_button);		
			Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons [l_button];
			w3x.Position = position;
			w3x.Expand = false;
			w3x.Fill = false;
		}

		public void SetPanelSize(Window window, SimpleImagePanel simpleimagepanel, HBox hbox, int maxPanelWidth, int maxPanelHeight, int imageW, int imageH, int minWinWidth = 0, int minWinHeight = 0)
		{		
			const int optionsWidth = 390;
			// general taskbar size in win_8.1
//			const int taskbarHeight = 90;
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

		public FileChooserDialog GetImageFileChooserDialog(bool selectMultiple, bool allowVideo, string alternativeTitle = null)
		{
			object[] o = new object[]{Language.I.L[16],ResponseType.Ok, 
				Language.I.L[17],ResponseType.Cancel};
			string title;

			if (alternativeTitle == null) {
				title = selectMultiple ? Language.I.L[39] : Language.I.L[2];
			} else {
				title = alternativeTitle;
			}

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
			if (allowVideo) {
				filter.AddPattern("*.mp4");
				filter.AddPattern("*.avi");
				filter.AddPattern("*.divx");
				filter.AddPattern("*.xvid");
				filter.AddPattern("*.mp4");
				filter.AddPattern("*.mpg");
				filter.AddPattern("*.mpeg");
				filter.AddPattern("*.m2v");
			}
			fc.Filter = filter;
			// fc.RemoveShortcutFolderUri (Environment.GetFolderPath(Environment.SpecialFolder.Recent));

			return fc;
		}

		public void CreateMenubarInToolbar(HBox hboxToolbarButtons, int position, string stockicon, string tooltipText,
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
			rootitem.TooltipText = tooltipText;


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

		public void CreateToolbarIconButton(HBox hboxToolbarButtons, int position, string stockicon, string tooltipText, OnToolbarBtnPressed pressed, string label = null)
		{
			Button l_button = new Button();
			l_button.Image = Image.LoadFromResource(stockicon);
			l_button.Visible = true;
			l_button.TooltipText = tooltipText;
			l_button.Label = label;
			l_button.Image.Visible = true;
			l_button.Pressed += new EventHandler (pressed);
			hboxToolbarButtons.Add (l_button);		
			Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons [l_button];
			w3x.Position = position;
			w3x.Expand = false;
			w3x.Fill = false;
		}

		public void CreateToolbarUpdateButtonByTimer(HBox hboxToolbarButtons, int position)
		{
//			Console.WriteLine("SERVER_VERSION_FLOAT:  " + Constants.I.SERVER_VERSION_FLOAT);

			// First update check in Constants.Init()
			if (Constants.I.SERVER_VERSION_FLOAT > Constants.I.VERSION_FLOAT){
				GuiHelper.I.CreateToolbarUpdateButton (hboxToolbarButtons, position, 
					"security-medium-2.png", Language.I.L[70], Constants.I.SERVER_VERSION_FLOAT);				
			}

			if (Constants.I.SERVER_VERSION_FLOAT != 0) {
				return;
			}

			// Second update check
			Constants.I.CheckUpdateAsThread ();

			GLib.TimeoutHandler timeoutHandler = () => {
				if (Constants.I.SERVER_VERSION_FLOAT > Constants.I.VERSION_FLOAT){
					GuiHelper.I.CreateToolbarUpdateButton (hboxToolbarButtons, position, 
						"security-medium-2.png", Language.I.L[70], Constants.I.SERVER_VERSION_FLOAT);	
				}
					
//				bool retValue = Constants.I.SERVER_VERSION_FLOAT == 0;
//				Console.WriteLine("SERVER_VERSION_FLOAT==0:  " + retValue);

//				if (retValue){
//					// Third and further update checks
//					Constants.I.CheckUpdateAsThread ();
//				}

				return /*retValue*/ false;
			};
			// waiting 5 seconds
			GLib.Timeout.Add(5 * 1000, timeoutHandler);			
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
			
		public void CreateDesktopcontextmenuLanguageAndInfoToolbarButtons(HBox hboxToolbarButtons, int position, OnToolbarBtnPressed languageChanged)
		{			
			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, position, "applications-system-3.png", Language.I.L[227], 
				(s, e) => { 
					new OptionsWidget ().Show(); 
//					Config.Save (Constants.I.CONFIG);
				});
			position++;

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, position, "folder-new-4.png", Language.I.L[59], 
				(s, e) => { 
					new AskForDesktopContextMenuWindow (false, Constants.I.CONFIG).Show(); 
					Config.Save (Constants.I.CONFIG);
				});
			position++;

			GuiHelper.I.CreateToolbarIconButton (	hboxToolbarButtons, 
				position, "tools-check-spelling-5.png", 				
				Language.I.L[43] +	": " + 
				Language.I.L[0] + Constants.N + Constants.N + 
				Language.I.L[44] +	": "+ Constants.N +
				Language.AllLanguagesAsString, 
				languageChanged);
			position++;

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, position, "help-about-3.png", Language.I.L[4], 
				(s, e) => { TroonieAboutDialog.I.Run (); });
			position++;

			GuiHelper.I.CreateToolbarUpdateButtonByTimer(hboxToolbarButtons, position);			
		}

        public void CreateSearchToolbar(
            HBox hboxToolbarButtons, 
            int position,
            string stockicon, 
            string tooltipText,
            out Entry searchEntry, 
            out Label searchLabel,
            out Button up_button,
            out Button down_button,
            EventHandler toolbarBtnSearchPressed,
            EventHandler searchEntryChanged,
            OnToolbarBtnPressed upArrowPressed,
            OnToolbarBtnPressed downArrowPressed) 
        {
            Button l_button = new Button();
            l_button.Image = Gtk.Image.LoadFromResource(stockicon);
            l_button.Visible = true;
            l_button.TooltipText = tooltipText;
            l_button.Image.Visible = true;
            l_button.Pressed += toolbarBtnSearchPressed;
            hboxToolbarButtons.Add(l_button);
            Box.BoxChild w3x = (Box.BoxChild)hboxToolbarButtons[l_button];
            w3x.Position = position;
            w3x.Expand = false;
            w3x.Fill = false;

            searchEntry = new Entry();
            //searchEntry.Visible = true;
            //searchEntry.WidthChars = 10;
            searchEntry.MaxLength = 30;
            searchEntry.Changed += searchEntryChanged;
            hboxToolbarButtons.Add(searchEntry);
            w3x = (Box.BoxChild)hboxToolbarButtons[searchEntry];
            w3x.Position = position + 1;
            w3x.Expand = false;
            w3x.Fill = false;

            up_button = new Button();
            up_button.Add(new Arrow(ArrowType.Up, ShadowType.Out) { Visible = true });
            //up_button.Visible = true;
            up_button.Pressed += new EventHandler(upArrowPressed);
            hboxToolbarButtons.Add(up_button);
            w3x = (Box.BoxChild)hboxToolbarButtons[up_button];
            w3x.Position = position + 2;
            w3x.Expand = false;
            w3x.Fill = false;

            down_button = new Button();
            down_button.Add(new Arrow(ArrowType.Down, ShadowType.Out) { Visible = true });
            //down_button.Visible = true;
            down_button.Pressed += new EventHandler(downArrowPressed);
            hboxToolbarButtons.Add(down_button);
            w3x = (Box.BoxChild)hboxToolbarButtons[down_button];
            w3x.Position = position + 3;
            w3x.Expand = false;
            w3x.Fill = false;

            searchLabel = new Label();
            //SetSearchLabel();
            //searchLabel.Visible = true;
            hboxToolbarButtons.Add(searchLabel);
            w3x = (Box.BoxChild)hboxToolbarButtons[searchLabel];
            w3x.Position = position + 4;
            w3x.Expand = false;
            w3x.Fill = false;
        }

        public string GetFileSizeString(long fileSize, int decimals)
		{
			double dl = fileSize;
			string dl_ext = Language.I.L[247];
			if (fileSize > 1024 * 1024) {
				dl = dl / (1024 * 1024);
				dl_ext = Language.I.L[249];
			}
			else if (fileSize > 1024) {
				dl = dl / 1024;
				dl_ext = Language.I.L[248];
			}
			dl = Math.Round(dl, decimals);
			return dl + " " + dl_ext;			
		}
	}
}

