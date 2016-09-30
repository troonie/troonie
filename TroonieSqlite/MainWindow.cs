using System;
using Gtk;
using Troonie_Lib;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using IOPath = System.IO.Path;
using TroonieSqlite;

internal partial class MainWindow: Gtk.Window
{
	private Sqlite_ColorConverter colorConverter;
	private string path;


	public MainWindow (List<string> pFilenames) : base (Gtk.WindowType.Toplevel)
	{
		Build ();

		if (Constants.I.WINDOWS) {
			Gtk.Drag.DestSet (this, 0, null, 0);
		} else {
			// Original is ShadowType.EtchedIn, but linux cannot draw it correctly.
			// Otherwise ShadowType.In looks terrible at Win10.
			frameImageFormat.ShadowType = ShadowType.In;
			frameImageResize.ShadowType = ShadowType.In;
			frameImageList.ShadowType = ShadowType.In;
			frameImageFormat.ShadowType = ShadowType.In;
			frameOutputDirectory.ShadowType = ShadowType.In;

			Gtk.Drag.DestSet (this, DestDefaults.All, MainClass.Target_table, Gdk.DragAction.Copy);

		}

		htlbOutputDirectory.InitDefaultValues ();
		htlbOutputDirectory.OnHyperTextLabelTextChanged += OnHyperTextLabelTextChanged;

		colorConverter = new Sqlite_ColorConverter ();
		SetGuiColors ();

		FillImageList (pFilenames);
	}

	private void SetGuiColors()
	{
		this.ModifyBg(StateType.Normal, colorConverter.GRID);
		eventboxRoot.ModifyBg(StateType.Normal, colorConverter.GRID);
		eventboxToolbar.ModifyBg(StateType.Normal, colorConverter.GRID);

		entryBiggerLength.ModifyBase(StateType.Normal, colorConverter.White);
		entryFixSizeWidth.ModifyBase(StateType.Normal, colorConverter.White);
		entryFixSizeHeight.ModifyBase(StateType.Normal, colorConverter.White);
		eventboxImageList.ModifyBg(StateType.Normal, colorConverter.White);

		lbFrameImageFormat.ModifyFg(StateType.Normal, colorConverter.FONT);
		lbFrameImageResize.ModifyFg(StateType.Normal, colorConverter.FONT);
		lbFrameOutputDirectory.ModifyFg(StateType.Normal, colorConverter.FONT);

		GtkLabel11.ModifyFg(StateType.Normal, colorConverter.FONT);
		GtkLabel15.ModifyFg(StateType.Normal, colorConverter.FONT);
		GtkLabel21.ModifyFg(StateType.Normal, colorConverter.FONT);
		GtkLabel29.ModifyFg(StateType.Normal, colorConverter.FONT);
		GtkLabel6.ModifyFg(StateType.Normal, colorConverter.FONT);
	}

	private void FillImageList(List<string> newImages)
	{
		Sqlite_PressedInButton l_pressedInButton;

		for (int i=0; i<newImages.Count; ++i)
		{
			string waste = Constants.I.WINDOWS ? "file:///" : "file://";
			newImages [i] = newImages [i].Replace (@waste, "");
			// Also change possible wrong directory separator
			newImages [i] = newImages [i].Replace (IOPath.AltDirectorySeparatorChar, IOPath.DirectorySeparatorChar);

			// check whether file is image or video
			FileInfo info = new FileInfo (newImages [i]);
			string ext = info.Extension.ToLower ();

			if (Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext) ||
				Constants.VideoExtensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext || x.Value.Item3 == ext)) {
				l_pressedInButton = new Sqlite_PressedInButton ();
				l_pressedInButton.FullText = newImages [i];
				l_pressedInButton.Text = newImages [i].Substring(newImages[i].LastIndexOf(
					IOPath.DirectorySeparatorChar) + 1);	
				l_pressedInButton.CanFocus = true;
				l_pressedInButton.Sensitive = true;
				l_pressedInButton.TextSize = 10;
				l_pressedInButton.ShowAll ();

				vboxImageList.PackStart(l_pressedInButton, false, false, 0);
			}					
		}	
	}

	#region drag and drop

	protected void OnDragDrop (object sender, Gtk.DragDropArgs args)
	{
		Gtk.Drag.GetData
		((Gtk.Widget)sender, args.Context,
			args.Context.Targets[0], args.Time);
	}

	protected void OnDragDataReceived (object sender, Gtk.DragDataReceivedArgs args)
	{
		if (args.SelectionData.Length > 0
			&& args.SelectionData.Format == 8) {

			byte[] data = args.SelectionData.Data;
			string encoded = System.Text.Encoding.UTF8.GetString (data);

			// drag n drop at linux wont accept spaces, so it has to be replaced
			encoded = encoded.Replace ("%20", " ");

			List<string> newImages = new List<string> (encoded.Split ('\r', '\n'));
			newImages.RemoveAll (string.IsNullOrEmpty);

			// I don't know what last object (when Windows) is,
			//  but I tested and noticed that it is not a path
			if (Constants.I.WINDOWS)
				newImages.RemoveAt (newImages.Count-1);

			FillImageList (newImages);
			newImages.Clear ();
		}
	}

	#endregion drag and drop

	protected void OnHyperTextLabelTextChanged()
	{
		path = htlbOutputDirectory.Text;
	}	

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
//		Dispose();

		// DO NOT QUIT here!
		Application.Quit ();
		a.RetVal = true;
	}
}
