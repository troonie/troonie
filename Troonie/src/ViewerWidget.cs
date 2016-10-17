using System;
using Troonie_Lib;
using System.Collections.Generic;
using Gtk;
using System.Drawing;
using ImageConverter = Troonie_Lib.ImageConverter;
using IOPath = System.IO.Path;
using System.IO;

namespace Troonie
{
	public partial class ViewerWidget : Gtk.Window
	{
		private const string blackFileName = "black.png";

		private Troonie.ColorConverter colorConverter = Troonie.ColorConverter.Instance;
		private Constants constants = Constants.I;
		private int imageW; 
		private int imageH;
		private string tempScaledImageFileName;

		public string FileName { get; set; }
		public BitmapWithTag bt;

		public ViewerWidget (List<string> newImages) :	base (Gtk.WindowType.Toplevel)
		{


			Build ();
			this.SetIconFromFile(Constants.I.EXEPATH + Constants.ICONNAME);

			if (newImages != null)
				FillImageList (newImages);

			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 0, "folder-new-3.png", Language.I.L[2], OnToolbarBtn_OpenPressed);
//			GuiHelper.I.CreateToolbarIconButton (hboxToolbarButtons, 1, "document-save-5.png", Language.I.L[3], OnToolbarBtn_SaveAsPressed);
//			GuiHelper.I.CreateToolbarSeparator (hboxToolbarButtons, 2);
//			GuiHelper.I.CreateDesktopcontextmenuLanguageAndInfoToolbarButtons (hboxToolbarButtons, 3, OnToolbarBtn_LanguagePressed);

			SetGuiColors ();
			SetLanguageToGui ();
			Initialize(true);

			if (constants.WINDOWS) {
				Gtk.Drag.DestSet (this, 0, null, 0);
			} else {
				// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
				// Otherwise ShadowType.In looks terrible at Win10.

//				frameCursorPos.ShadowType = ShadowType.In;
//				frameSteganography.ShadowType = ShadowType.In;
//				frameModus.ShadowType = ShadowType.In;
//				frameKey.ShadowType = ShadowType.In;
//				frameContent.ShadowType = ShadowType.In;
				Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);
			}

//			simpleimagepanel1.OnCursorPosChanged += OnCursorPosChanged;

			if (Constants.I.CONFIG.AskForDesktopContextMenu) {
				new AskForDesktopContextMenuWindow (true, Constants.I.CONFIG).Show ();
			}					
		}

		public override void Destroy ()
		{
			if (bt != null) {
				bt.Dispose ();
			}
			base.Destroy ();
		}

		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			if (bt != null) {
				bt.Dispose ();
			}
			this.DestroyAll ();

			Application.Quit ();
			a.RetVal = true;

//			File.Delete (tempScaledImageFileName);
//			File.Delete (Constants.I.EXEPATH + blackFileName);
		}

		private void LoadException()
		{
			FileName = null;
			Initialize (true);

			MessageDialog md = new MessageDialog (
				null, DialogFlags.Modal, 
				MessageType.Info, 
				ButtonsType.Ok, Language.I.L[51]);
			md.Run ();
			md.Destroy ();		
		}

		private void Initialize(bool newFileName)
		{

//			GuiHelper.I.SetPanelSize(this, simpleimagepanel, hbox1, 500, 600, imageW, imageH, 1200, 650);

//			simpleimagepanel1.Initialize();

			ShowAll();
		}		

		private void SetGuiColors()
		{
			this.ModifyBg(StateType.Normal, colorConverter.GRID);
			eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

//			lbFrameCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbCursorPos.ModifyFg (StateType.Normal, colorConverter.FONT);
//
//			lbFrameSteganography.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameModus.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameKey.ModifyFg (StateType.Normal, colorConverter.FONT);
//			lbFrameContent.ModifyFg (StateType.Normal, colorConverter.FONT);
		}

		private void SetLanguageToGui()
		{
//			lbFrameCursorPos.LabelProp = "<b>" + Language.I.L[15] + "</b>";
//			btnOk.Text = Language.I.L[16];
//			btnOk.Redraw ();
//
		}


		private void FillImageList(List<string> newImages)
		{
//			PressedInButton l_pressedInButton;
//
//			for (int i=0; i<newImages.Count; ++i)
//			{
//				string waste = Constants.I.WINDOWS ? "file:///" : "file://";
//				newImages [i] = newImages [i].Replace (@waste, "");
//				// Also change possible wrong directory separator
//				newImages [i] = newImages [i].Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);
//
//				// check whether file is image or video
//				FileInfo info = new FileInfo (newImages [i]);
//				string ext = info.Extension.ToLower ();
//
//				if (ext.Length != 0 && (Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext) ||
//					Constants.VideoExtensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext || x.Value.Item3 == ext))) {
//					l_pressedInButton = new PressedInButton ();
//					l_pressedInButton.FullText = newImages [i];
//					l_pressedInButton.Text = newImages [i].Substring(newImages[i].LastIndexOf(
//						IOPath.DirectorySeparatorChar) + 1);	
//					l_pressedInButton.CanFocus = true;
//					l_pressedInButton.Sensitive = true;
//					l_pressedInButton.TextSize = 10;
//					l_pressedInButton.ShowAll ();
//
//					vboxImageList.PackStart(l_pressedInButton, false, false, 0);
//				}					
//			}	

			int biggestLength100 = 300;
			uint n = 0;
			foreach (string s in newImages) {
				
				string path = IOPath.GetDirectoryName (s) + IOPath.DirectorySeparatorChar + "thumb" + 
					biggestLength100.ToString() + IOPath.DirectorySeparatorChar;
				Directory.CreateDirectory (path);
				string relativeImageName = s.Substring(s.LastIndexOf(IOPath.DirectorySeparatorChar) + 1);
				relativeImageName = relativeImageName.Substring(0, relativeImageName.LastIndexOf('.'));
				relativeImageName = relativeImageName + Constants.Extensions[TroonieImageFormat.PNG24].Item1;

				if (!File.Exists (path + relativeImageName)) {
					BitmapWithTag bt = new BitmapWithTag (s, true);
					Config c = new Config ();
					c.BiggestLength = biggestLength100;
					c.FileOverwriting = false;
					c.Path = path;
					c.Format = TroonieImageFormat.PNG24;
					c.ResizeVersion = ResizeVersion.BiggestLength;

					// TODO: Catch, what should be happen, when success==false
					bool success = bt.Save (c, relativeImageName);
					bt.Dispose ();
				}
			
				ViewerImagePanel2 vip2 = new ViewerImagePanel2 ();
				vip2.SurfaceFileName = path + relativeImageName;
				vip2.WidthRequest = biggestLength100;
				vip2.HeightRequest = biggestLength100;
				vip2.Initialize ();
				//			vip.SimpleImagePanel.ShowAll ();
				tableViewer.Attach (vip2, n, n + 1, 0, 1); //PackStart(l_pressedInButton, false, false, 0);
				n++;
			}
				
//			ViewerImagePanel vip2 = new ViewerImagePanel ();
//			vip2.SimpleImagePanel.SurfaceFileName = newImages[0];
//			vip2.SimpleImagePanel.WidthRequest = 200;
//			vip2.SimpleImagePanel.HeightRequest = 100;
//			vip2.SimpleImagePanel.Initialize ();
//			tableViewer.Attach (vip2, 1, 2, 1, 2);

		}

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
	}
}

