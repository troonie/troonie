
// This file has been generated by the GUI designer. Do not modify.
namespace Picturez
{
	public partial class SaveAsDialog
	{
		private global::Gtk.Frame frame7;
		private global::Gtk.Alignment GtkAlignment10;
		private global::Gtk.VBox vbox6;
		private global::Gtk.HBox hbox4;
		private global::Gtk.Label lbDirectoryText;
		private global::Picturez.HyperTextLabel htLabelDirectory;
		private global::Gtk.HBox hbox3;
		private global::Gtk.Label label3;
		private global::Gtk.Entry entryFilename;
		private global::Gtk.Label lbFormat;
		private global::Gtk.Label lbFile;
		private global::Gtk.Frame frame3;
		private global::Gtk.Alignment GtkAlignment4;
		private global::Gtk.VBox vbox2;
		private global::Gtk.HBox hbox1;
		private global::Gtk.RadioButton rdJpeg;
		private global::Gtk.Label lbQuality;
		private global::Gtk.HScale hscaleQuality;
		private global::Gtk.RadioButton rdJpegGray;
		private global::Gtk.Label GtkLabel3;
		private global::Gtk.Frame frame4;
		private global::Gtk.Alignment GtkAlignment5;
		private global::Gtk.VBox vbox3;
		private global::Gtk.RadioButton rdPng1bit;
		private global::Gtk.RadioButton rdPng8Bit;
		private global::Gtk.RadioButton rdPng24Bit;
		private global::Gtk.HBox hbox2;
		private global::Gtk.RadioButton rdPNG32bit;
		private global::Gtk.Label lbTransparencyColor;
		private global::Gtk.ColorButton btnColor;
		private global::Gtk.Label GtkLabel7;
		private global::Gtk.Frame frame5;
		private global::Gtk.Alignment GtkAlignment6;
		private global::Gtk.VBox vbox4;
		private global::Gtk.RadioButton rdBmp1bit;
		private global::Gtk.RadioButton rdBmp8bit;
		private global::Gtk.RadioButton rdBmp24bit;
		private global::Gtk.Label GtkLabel11;
		private global::Gtk.Frame frame6;
		private global::Gtk.Alignment GtkAlignment7;
		private global::Gtk.VBox vbox5;
		private global::Gtk.RadioButton rdTiff;
		private global::Gtk.RadioButton rdGif;
		private global::Gtk.RadioButton rdWmf;
		private global::Gtk.RadioButton rdEmf;
		private global::Gtk.RadioButton rdIcon;
		private global::Gtk.Label GtkLabel17;
		private global::Gtk.Button buttonCancel;
		private global::Gtk.Button buttonOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Picturez.SaveAsDialog
			this.Name = "Picturez.SaveAsDialog";
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Internal child Picturez.SaveAsDialog.VBox
			global::Gtk.VBox w1 = this.VBox;
			w1.Name = "dialog1_VBox";
			w1.BorderWidth = ((uint)(2));
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.frame7 = new global::Gtk.Frame ();
			this.frame7.Name = "frame7";
			this.frame7.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame7.Gtk.Container+ContainerChild
			this.GtkAlignment10 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment10.Name = "GtkAlignment10";
			this.GtkAlignment10.LeftPadding = ((uint)(12));
			// Container child GtkAlignment10.Gtk.Container+ContainerChild
			this.vbox6 = new global::Gtk.VBox ();
			this.vbox6.Name = "vbox6";
			this.vbox6.Spacing = 6;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbox4 = new global::Gtk.HBox ();
			this.hbox4.Name = "hbox4";
			this.hbox4.Spacing = 6;
			// Container child hbox4.Gtk.Box+BoxChild
			this.lbDirectoryText = new global::Gtk.Label ();
			this.lbDirectoryText.Name = "lbDirectoryText";
			this.lbDirectoryText.LabelProp = global::Mono.Unix.Catalog.GetString ("Directory");
			this.hbox4.Add (this.lbDirectoryText);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.lbDirectoryText]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox4.Gtk.Box+BoxChild
			this.htLabelDirectory = new global::Picturez.HyperTextLabel ();
			this.htLabelDirectory.Sensitive = false;
			this.htLabelDirectory.Name = "htLabelDirectory";
			this.htLabelDirectory.Sensitive = false;
			this.htLabelDirectory.Font = "Serif";
			this.htLabelDirectory.Text = "Test";
			this.htLabelDirectory.TextSize = 10;
			this.htLabelDirectory.ShownTextLength = 50;
			this.htLabelDirectory.Underline = true;
			this.htLabelDirectory.Bold = true;
			this.htLabelDirectory.Italic = false;
			this.hbox4.Add (this.htLabelDirectory);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.htLabelDirectory]));
			w3.Position = 1;
			this.vbox6.Add (this.hbox4);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbox4]));
			w4.Position = 0;
			w4.Expand = false;
			w4.Fill = false;
			// Container child vbox6.Gtk.Box+BoxChild
			this.hbox3 = new global::Gtk.HBox ();
			this.hbox3.Name = "hbox3";
			this.hbox3.Spacing = 6;
			// Container child hbox3.Gtk.Box+BoxChild
			this.label3 = new global::Gtk.Label ();
			this.label3.Name = "label3";
			this.label3.LabelProp = global::Mono.Unix.Catalog.GetString ("File name");
			this.hbox3.Add (this.label3);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.label3]));
			w5.Position = 0;
			w5.Expand = false;
			w5.Fill = false;
			// Container child hbox3.Gtk.Box+BoxChild
			this.entryFilename = new global::Gtk.Entry ();
			this.entryFilename.CanFocus = true;
			this.entryFilename.Name = "entryFilename";
			this.entryFilename.IsEditable = true;
			this.entryFilename.InvisibleChar = '●';
			this.hbox3.Add (this.entryFilename);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.entryFilename]));
			w6.Position = 1;
			// Container child hbox3.Gtk.Box+BoxChild
			this.lbFormat = new global::Gtk.Label ();
			this.lbFormat.Name = "lbFormat";
			this.lbFormat.LabelProp = global::Mono.Unix.Catalog.GetString (".jpg");
			this.hbox3.Add (this.lbFormat);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.lbFormat]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox6.Add (this.hbox3);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox6 [this.hbox3]));
			w8.Position = 1;
			w8.Expand = false;
			w8.Fill = false;
			this.GtkAlignment10.Add (this.vbox6);
			this.frame7.Add (this.GtkAlignment10);
			this.lbFile = new global::Gtk.Label ();
			this.lbFile.Name = "lbFile";
			this.lbFile.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>File</b>");
			this.lbFile.UseMarkup = true;
			this.frame7.LabelWidget = this.lbFile;
			w1.Add (this.frame7);
			global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(w1 [this.frame7]));
			w11.Position = 0;
			w11.Expand = false;
			w11.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.frame3 = new global::Gtk.Frame ();
			this.frame3.Name = "frame3";
			this.frame3.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame3.Gtk.Container+ContainerChild
			this.GtkAlignment4 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment4.Name = "GtkAlignment4";
			this.GtkAlignment4.LeftPadding = ((uint)(12));
			// Container child GtkAlignment4.Gtk.Container+ContainerChild
			this.vbox2 = new global::Gtk.VBox ();
			this.vbox2.Name = "vbox2";
			this.vbox2.Spacing = 6;
			// Container child vbox2.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.rdJpeg = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("JPEG"));
			this.rdJpeg.CanFocus = true;
			this.rdJpeg.Name = "rdJpeg";
			this.rdJpeg.DrawIndicator = true;
			this.rdJpeg.UseUnderline = true;
			this.rdJpeg.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.hbox1.Add (this.rdJpeg);
			global::Gtk.Box.BoxChild w12 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.rdJpeg]));
			w12.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.lbQuality = new global::Gtk.Label ();
			this.lbQuality.Name = "lbQuality";
			this.lbQuality.LabelProp = global::Mono.Unix.Catalog.GetString ("Quality");
			this.hbox1.Add (this.lbQuality);
			global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.lbQuality]));
			w13.Position = 1;
			w13.Expand = false;
			w13.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.hscaleQuality = new global::Gtk.HScale (null);
			this.hscaleQuality.CanFocus = true;
			this.hscaleQuality.Name = "hscaleQuality";
			this.hscaleQuality.Adjustment.Lower = 1;
			this.hscaleQuality.Adjustment.Upper = 100;
			this.hscaleQuality.Adjustment.PageIncrement = 10;
			this.hscaleQuality.Adjustment.StepIncrement = 1;
			this.hscaleQuality.Adjustment.Value = 90;
			this.hscaleQuality.DrawValue = true;
			this.hscaleQuality.Digits = 0;
			this.hscaleQuality.ValuePos = ((global::Gtk.PositionType)(2));
			this.hbox1.Add (this.hscaleQuality);
			global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.hscaleQuality]));
			w14.Position = 2;
			this.vbox2.Add (this.hbox1);
			global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.hbox1]));
			w15.Position = 0;
			w15.Expand = false;
			w15.Fill = false;
			// Container child vbox2.Gtk.Box+BoxChild
			this.rdJpegGray = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("JPEG (grayscale)"));
			this.rdJpegGray.CanFocus = true;
			this.rdJpegGray.Name = "rdJpegGray";
			this.rdJpegGray.DrawIndicator = true;
			this.rdJpegGray.UseUnderline = true;
			this.rdJpegGray.Group = this.rdJpeg.Group;
			this.vbox2.Add (this.rdJpegGray);
			global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.vbox2 [this.rdJpegGray]));
			w16.Position = 1;
			w16.Expand = false;
			w16.Fill = false;
			this.GtkAlignment4.Add (this.vbox2);
			this.frame3.Add (this.GtkAlignment4);
			this.GtkLabel3 = new global::Gtk.Label ();
			this.GtkLabel3.Name = "GtkLabel3";
			this.GtkLabel3.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>JPEG</b>");
			this.GtkLabel3.UseMarkup = true;
			this.frame3.LabelWidget = this.GtkLabel3;
			w1.Add (this.frame3);
			global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(w1 [this.frame3]));
			w19.Position = 1;
			w19.Expand = false;
			w19.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.frame4 = new global::Gtk.Frame ();
			this.frame4.Name = "frame4";
			this.frame4.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame4.Gtk.Container+ContainerChild
			this.GtkAlignment5 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment5.Name = "GtkAlignment5";
			this.GtkAlignment5.LeftPadding = ((uint)(12));
			// Container child GtkAlignment5.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rdPng1bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("1 Bit PNG blackwhite"));
			this.rdPng1bit.Sensitive = false;
			this.rdPng1bit.CanFocus = true;
			this.rdPng1bit.Name = "rdPng1bit";
			this.rdPng1bit.DrawIndicator = true;
			this.rdPng1bit.UseUnderline = true;
			this.rdPng1bit.Group = this.rdJpeg.Group;
			this.vbox3.Add (this.rdPng1bit);
			global::Gtk.Box.BoxChild w20 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rdPng1bit]));
			w20.Position = 0;
			w20.Expand = false;
			w20.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rdPng8Bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("8 Bit PNG grayscale"));
			this.rdPng8Bit.CanFocus = true;
			this.rdPng8Bit.Name = "rdPng8Bit";
			this.rdPng8Bit.DrawIndicator = true;
			this.rdPng8Bit.UseUnderline = true;
			this.rdPng8Bit.Group = this.rdJpeg.Group;
			this.vbox3.Add (this.rdPng8Bit);
			global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rdPng8Bit]));
			w21.Position = 1;
			w21.Expand = false;
			w21.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.rdPng24Bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("24 Bit PNG"));
			this.rdPng24Bit.CanFocus = true;
			this.rdPng24Bit.Name = "rdPng24Bit";
			this.rdPng24Bit.DrawIndicator = true;
			this.rdPng24Bit.UseUnderline = true;
			this.rdPng24Bit.Group = this.rdJpeg.Group;
			this.vbox3.Add (this.rdPng24Bit);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.rdPng24Bit]));
			w22.Position = 2;
			w22.Expand = false;
			w22.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.rdPNG32bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("32 Bit PNG with transparency"));
			this.rdPNG32bit.CanFocus = true;
			this.rdPNG32bit.Name = "rdPNG32bit";
			this.rdPNG32bit.DrawIndicator = true;
			this.rdPNG32bit.UseUnderline = true;
			this.rdPNG32bit.Group = this.rdJpeg.Group;
			this.hbox2.Add (this.rdPNG32bit);
			global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.rdPNG32bit]));
			w23.Position = 0;
			// Container child hbox2.Gtk.Box+BoxChild
			this.lbTransparencyColor = new global::Gtk.Label ();
			this.lbTransparencyColor.Sensitive = false;
			this.lbTransparencyColor.Name = "lbTransparencyColor";
			this.lbTransparencyColor.LabelProp = global::Mono.Unix.Catalog.GetString ("Transparancy color");
			this.hbox2.Add (this.lbTransparencyColor);
			global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.lbTransparencyColor]));
			w24.Position = 1;
			w24.Expand = false;
			w24.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.btnColor = new global::Gtk.ColorButton ();
			this.btnColor.Sensitive = false;
			this.btnColor.CanFocus = true;
			this.btnColor.Events = ((global::Gdk.EventMask)(784));
			this.btnColor.Name = "btnColor";
			this.hbox2.Add (this.btnColor);
			global::Gtk.Box.BoxChild w25 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.btnColor]));
			w25.Position = 2;
			w25.Expand = false;
			w25.Fill = false;
			this.vbox3.Add (this.hbox2);
			global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox2]));
			w26.Position = 3;
			w26.Expand = false;
			w26.Fill = false;
			this.GtkAlignment5.Add (this.vbox3);
			this.frame4.Add (this.GtkAlignment5);
			this.GtkLabel7 = new global::Gtk.Label ();
			this.GtkLabel7.Name = "GtkLabel7";
			this.GtkLabel7.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>PNG</b>");
			this.GtkLabel7.UseMarkup = true;
			this.frame4.LabelWidget = this.GtkLabel7;
			w1.Add (this.frame4);
			global::Gtk.Box.BoxChild w29 = ((global::Gtk.Box.BoxChild)(w1 [this.frame4]));
			w29.Position = 2;
			w29.Expand = false;
			w29.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.frame5 = new global::Gtk.Frame ();
			this.frame5.Name = "frame5";
			this.frame5.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame5.Gtk.Container+ContainerChild
			this.GtkAlignment6 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment6.Name = "GtkAlignment6";
			this.GtkAlignment6.LeftPadding = ((uint)(12));
			// Container child GtkAlignment6.Gtk.Container+ContainerChild
			this.vbox4 = new global::Gtk.VBox ();
			this.vbox4.Name = "vbox4";
			this.vbox4.Spacing = 6;
			// Container child vbox4.Gtk.Box+BoxChild
			this.rdBmp1bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("1 Bit BMP"));
			this.rdBmp1bit.Sensitive = false;
			this.rdBmp1bit.CanFocus = true;
			this.rdBmp1bit.Name = "rdBmp1bit";
			this.rdBmp1bit.DrawIndicator = true;
			this.rdBmp1bit.UseUnderline = true;
			this.rdBmp1bit.Group = this.rdJpeg.Group;
			this.vbox4.Add (this.rdBmp1bit);
			global::Gtk.Box.BoxChild w30 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.rdBmp1bit]));
			w30.Position = 0;
			w30.Expand = false;
			w30.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.rdBmp8bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("8 Bit BMP grayscale"));
			this.rdBmp8bit.CanFocus = true;
			this.rdBmp8bit.Name = "rdBmp8bit";
			this.rdBmp8bit.DrawIndicator = true;
			this.rdBmp8bit.UseUnderline = true;
			this.rdBmp8bit.Group = this.rdJpeg.Group;
			this.vbox4.Add (this.rdBmp8bit);
			global::Gtk.Box.BoxChild w31 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.rdBmp8bit]));
			w31.Position = 1;
			w31.Expand = false;
			w31.Fill = false;
			// Container child vbox4.Gtk.Box+BoxChild
			this.rdBmp24bit = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("24 Bit BMP"));
			this.rdBmp24bit.CanFocus = true;
			this.rdBmp24bit.Name = "rdBmp24bit";
			this.rdBmp24bit.DrawIndicator = true;
			this.rdBmp24bit.UseUnderline = true;
			this.rdBmp24bit.Group = this.rdJpeg.Group;
			this.vbox4.Add (this.rdBmp24bit);
			global::Gtk.Box.BoxChild w32 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.rdBmp24bit]));
			w32.Position = 2;
			w32.Expand = false;
			w32.Fill = false;
			this.GtkAlignment6.Add (this.vbox4);
			this.frame5.Add (this.GtkAlignment6);
			this.GtkLabel11 = new global::Gtk.Label ();
			this.GtkLabel11.Name = "GtkLabel11";
			this.GtkLabel11.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>BMP</b>");
			this.GtkLabel11.UseMarkup = true;
			this.frame5.LabelWidget = this.GtkLabel11;
			w1.Add (this.frame5);
			global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(w1 [this.frame5]));
			w35.Position = 3;
			w35.Expand = false;
			w35.Fill = false;
			// Container child dialog1_VBox.Gtk.Box+BoxChild
			this.frame6 = new global::Gtk.Frame ();
			this.frame6.Name = "frame6";
			this.frame6.ShadowType = ((global::Gtk.ShadowType)(0));
			// Container child frame6.Gtk.Container+ContainerChild
			this.GtkAlignment7 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment7.Name = "GtkAlignment7";
			this.GtkAlignment7.LeftPadding = ((uint)(12));
			// Container child GtkAlignment7.Gtk.Container+ContainerChild
			this.vbox5 = new global::Gtk.VBox ();
			this.vbox5.Name = "vbox5";
			this.vbox5.Spacing = 6;
			// Container child vbox5.Gtk.Box+BoxChild
			this.rdTiff = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("TIFF"));
			this.rdTiff.CanFocus = true;
			this.rdTiff.Name = "rdTiff";
			this.rdTiff.DrawIndicator = true;
			this.rdTiff.UseUnderline = true;
			this.rdTiff.Group = this.rdJpeg.Group;
			this.vbox5.Add (this.rdTiff);
			global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.rdTiff]));
			w36.Position = 0;
			w36.Expand = false;
			w36.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.rdGif = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("GIF"));
			this.rdGif.CanFocus = true;
			this.rdGif.Name = "rdGif";
			this.rdGif.DrawIndicator = true;
			this.rdGif.UseUnderline = true;
			this.rdGif.Group = this.rdJpeg.Group;
			this.vbox5.Add (this.rdGif);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.rdGif]));
			w37.Position = 1;
			w37.Expand = false;
			w37.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.rdWmf = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("WMF"));
			this.rdWmf.CanFocus = true;
			this.rdWmf.Name = "rdWmf";
			this.rdWmf.DrawIndicator = true;
			this.rdWmf.UseUnderline = true;
			this.rdWmf.Group = this.rdJpeg.Group;
			this.vbox5.Add (this.rdWmf);
			global::Gtk.Box.BoxChild w38 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.rdWmf]));
			w38.Position = 2;
			w38.Expand = false;
			w38.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.rdEmf = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("EMF"));
			this.rdEmf.CanFocus = true;
			this.rdEmf.Name = "rdEmf";
			this.rdEmf.DrawIndicator = true;
			this.rdEmf.UseUnderline = true;
			this.rdEmf.Group = this.rdJpeg.Group;
			this.vbox5.Add (this.rdEmf);
			global::Gtk.Box.BoxChild w39 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.rdEmf]));
			w39.Position = 3;
			w39.Expand = false;
			w39.Fill = false;
			// Container child vbox5.Gtk.Box+BoxChild
			this.rdIcon = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("ICON"));
			this.rdIcon.CanFocus = true;
			this.rdIcon.Name = "rdIcon";
			this.rdIcon.DrawIndicator = true;
			this.rdIcon.UseUnderline = true;
			this.rdIcon.Group = this.rdJpeg.Group;
			this.vbox5.Add (this.rdIcon);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.vbox5 [this.rdIcon]));
			w40.Position = 4;
			w40.Expand = false;
			w40.Fill = false;
			this.GtkAlignment7.Add (this.vbox5);
			this.frame6.Add (this.GtkAlignment7);
			this.GtkLabel17 = new global::Gtk.Label ();
			this.GtkLabel17.Name = "GtkLabel17";
			this.GtkLabel17.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Other</b>");
			this.GtkLabel17.UseMarkup = true;
			this.frame6.LabelWidget = this.GtkLabel17;
			w1.Add (this.frame6);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(w1 [this.frame6]));
			w43.Position = 4;
			w43.Expand = false;
			w43.Fill = false;
			// Internal child Picturez.SaveAsDialog.ActionArea
			global::Gtk.HButtonBox w44 = this.ActionArea;
			w44.Name = "dialog1_ActionArea";
			w44.Spacing = 10;
			w44.BorderWidth = ((uint)(5));
			w44.LayoutStyle = ((global::Gtk.ButtonBoxStyle)(4));
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonCancel = new global::Gtk.Button ();
			this.buttonCancel.CanDefault = true;
			this.buttonCancel.CanFocus = true;
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.UseStock = true;
			this.buttonCancel.UseUnderline = true;
			this.buttonCancel.Label = "gtk-cancel";
			this.AddActionWidget (this.buttonCancel, -6);
			global::Gtk.ButtonBox.ButtonBoxChild w45 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w44 [this.buttonCancel]));
			w45.Expand = false;
			w45.Fill = false;
			// Container child dialog1_ActionArea.Gtk.ButtonBox+ButtonBoxChild
			this.buttonOk = new global::Gtk.Button ();
			this.buttonOk.CanDefault = true;
			this.buttonOk.CanFocus = true;
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.UseStock = true;
			this.buttonOk.UseUnderline = true;
			this.buttonOk.Label = "gtk-ok";
			this.AddActionWidget (this.buttonOk, -5);
			global::Gtk.ButtonBox.ButtonBoxChild w46 = ((global::Gtk.ButtonBox.ButtonBoxChild)(w44 [this.buttonOk]));
			w46.Position = 1;
			w46.Expand = false;
			w46.Fill = false;
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 400;
			this.DefaultHeight = 612;
			this.Show ();
			this.entryFilename.Changed += new global::System.EventHandler (this.OnEntryFilenameChanged);
			this.rdJpeg.Toggled += new global::System.EventHandler (this.OnRdJpegToggled);
			this.hscaleQuality.ValueChanged += new global::System.EventHandler (this.OnHscaleQualityValueChanged);
			this.rdJpegGray.Toggled += new global::System.EventHandler (this.OnRdJpegGrayToggled);
			this.rdPng1bit.Toggled += new global::System.EventHandler (this.OnRdPng1bitToggled);
			this.rdPng8Bit.Toggled += new global::System.EventHandler (this.OnRdPng8BitToggled);
			this.rdPng24Bit.Toggled += new global::System.EventHandler (this.OnRdPng24BitToggled);
			this.rdPNG32bit.Toggled += new global::System.EventHandler (this.OnRdPNG32bitToggled);
			this.btnColor.ColorSet += new global::System.EventHandler (this.OnBtnColorColorSet);
			this.rdBmp1bit.Toggled += new global::System.EventHandler (this.OnRdBmp1bitToggled);
			this.rdBmp8bit.Toggled += new global::System.EventHandler (this.OnRdBmp8bitToggled);
			this.rdBmp24bit.Toggled += new global::System.EventHandler (this.OnRdBmp24bitToggled);
			this.rdTiff.Toggled += new global::System.EventHandler (this.OnRdTiffToggled);
			this.rdGif.Toggled += new global::System.EventHandler (this.OnRdGifToggled);
			this.rdWmf.Toggled += new global::System.EventHandler (this.OnRdWmfToggled);
			this.rdEmf.Toggled += new global::System.EventHandler (this.OnRdEmfToggled);
			this.rdIcon.Toggled += new global::System.EventHandler (this.OnRdIconToggled);
		}
	}
}
