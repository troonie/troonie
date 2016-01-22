using System;
using Gtk;

namespace Picturez
{
	public delegate void OnToolbarBtnPressed (object sender, EventArgs e);

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

		public FileChooserDialog GetImageFileChooserDialog(bool selectMultiple)
		{
			object[] o = new object[]{"Cancel",ResponseType.Cancel,
				"OK",ResponseType.Ok};

			FileChooserDialog fc =
				new FileChooserDialog("Choose image files",
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

