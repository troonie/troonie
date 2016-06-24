
// This file has been generated by the GUI designer. Do not modify.
namespace Picturez
{
	public partial class StitchWidget
	{
		private global::Gtk.VBox vboxA;
		
		private global::Gtk.EventBox eventboxToolbar;
		
		private global::Gtk.HBox hbox1;
		
		private global::Picturez.SimpleImagePanel simpleimagepanel1;
		
		private global::Gtk.VBox vbox1;
		
		private global::Gtk.Frame frameStitch;
		
		private global::Gtk.Alignment AlignmentFrameStitch;
		
		private global::Gtk.VBox vboxStitch;
		
		private global::Gtk.Frame frameModus;
		
		private global::Gtk.Alignment AlignmentFrameModus;
		
		private global::Gtk.HBox hboxModus;
		
		private global::Gtk.RadioButton rdBtnLandscape;
		
		private global::Gtk.RadioButton rdBtnPortrait;
		
		private global::Gtk.Label lbFrameModus;
		
		private global::Gtk.Frame frameImagePositions;
		
		private global::Gtk.Alignment AlignmentImagePositions;
		
		private global::Gtk.Table tableImagePosition1;
		
		private global::Picturez.PicturezButton btn01BottomMinus;
		
		private global::Picturez.PicturezButton btn01BottomPlus;
		
		private global::Picturez.PicturezButton btn01LeftMinus;
		
		private global::Picturez.PicturezButton btn01LeftPlus;
		
		private global::Picturez.PicturezButton btn01RightMinus;
		
		private global::Picturez.PicturezButton btn01RightPlus;
		
		private global::Picturez.PicturezButton btn01TopMinus;
		
		private global::Picturez.PicturezButton btn01TopPlus;
		
		private global::Gtk.Label lb01Bottom;
		
		private global::Gtk.Label lb01Left;
		
		private global::Gtk.Label lb01Right;
		
		private global::Gtk.Label lb01Top;
		
		private global::Gtk.Label lbFrameImagePositions;
		
		private global::Gtk.Frame frameImagePositions2;
		
		private global::Gtk.Alignment AlignmentImagePositions1;
		
		private global::Gtk.Table tableImagePosition2;
		
		private global::Picturez.PicturezButton btn02BottomMinus;
		
		private global::Picturez.PicturezButton btn02BottomPlus;
		
		private global::Picturez.PicturezButton btn02LeftMinus;
		
		private global::Picturez.PicturezButton btn02LeftPlus;
		
		private global::Picturez.PicturezButton btn02RightMinus;
		
		private global::Picturez.PicturezButton btn02RightPlus;
		
		private global::Picturez.PicturezButton btn02TopMinus;
		
		private global::Picturez.PicturezButton btn02TopPlus;
		
		private global::Gtk.Label lb02Bottom;
		
		private global::Gtk.Label lb02Left;
		
		private global::Gtk.Label lb02Right;
		
		private global::Gtk.Label lb02Top;
		
		private global::Gtk.Label lbFrameImagePositions2;
		
		private global::Gtk.Label lbFrameStitch;
		
		private global::Gtk.Frame frameImageResolution;
		
		private global::Gtk.Alignment GtkAlignment2;
		
		private global::Gtk.Label lbImageResolution;
		
		private global::Gtk.Label lbFrameImageResolution;
		
		private global::Gtk.Frame frameCursorPos;
		
		private global::Gtk.Alignment GtkAlignment1;
		
		private global::Gtk.Label lbCursorPos;
		
		private global::Gtk.Label lbFrameCursorPos;
		
		private global::Picturez.PicturezButton btnOk;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Picturez.StitchWidget
			this.Name = "Picturez.StitchWidget";
			this.Title = global::Mono.Unix.Catalog.GetString ("StitchWidget");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Picturez.StitchWidget.Gtk.Container+ContainerChild
			this.vboxA = new global::Gtk.VBox ();
			this.vboxA.Name = "vboxA";
			this.vboxA.Spacing = 6;
			// Container child vboxA.Gtk.Box+BoxChild
			this.eventboxToolbar = new global::Gtk.EventBox ();
			this.eventboxToolbar.Name = "eventboxToolbar";
			this.vboxA.Add (this.eventboxToolbar);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vboxA [this.eventboxToolbar]));
			w1.Position = 0;
			w1.Expand = false;
			// Container child vboxA.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.simpleimagepanel1 = new global::Picturez.SimpleImagePanel ();
			this.simpleimagepanel1.Events = ((global::Gdk.EventMask)(256));
			this.simpleimagepanel1.Name = "simpleimagepanel1";
			this.simpleimagepanel1.ScaleCursorX = 0F;
			this.simpleimagepanel1.ScaleCursorY = 0F;
			this.hbox1.Add (this.simpleimagepanel1);
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.simpleimagepanel1]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox1.Gtk.Box+BoxChild
			this.vbox1 = new global::Gtk.VBox ();
			this.vbox1.Name = "vbox1";
			this.vbox1.Spacing = 6;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frameStitch = new global::Gtk.Frame ();
			this.frameStitch.Name = "frameStitch";
			// Container child frameStitch.Gtk.Container+ContainerChild
			this.AlignmentFrameStitch = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentFrameStitch.Name = "AlignmentFrameStitch";
			this.AlignmentFrameStitch.LeftPadding = ((uint)(12));
			this.AlignmentFrameStitch.RightPadding = ((uint)(12));
			this.AlignmentFrameStitch.BottomPadding = ((uint)(12));
			// Container child AlignmentFrameStitch.Gtk.Container+ContainerChild
			this.vboxStitch = new global::Gtk.VBox ();
			this.vboxStitch.Name = "vboxStitch";
			this.vboxStitch.Spacing = 6;
			// Container child vboxStitch.Gtk.Box+BoxChild
			this.frameModus = new global::Gtk.Frame ();
			this.frameModus.Name = "frameModus";
			// Container child frameModus.Gtk.Container+ContainerChild
			this.AlignmentFrameModus = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentFrameModus.Name = "AlignmentFrameModus";
			this.AlignmentFrameModus.LeftPadding = ((uint)(12));
			// Container child AlignmentFrameModus.Gtk.Container+ContainerChild
			this.hboxModus = new global::Gtk.HBox ();
			this.hboxModus.Name = "hboxModus";
			this.hboxModus.Spacing = 6;
			// Container child hboxModus.Gtk.Box+BoxChild
			this.rdBtnLandscape = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Landscape"));
			this.rdBtnLandscape.CanFocus = true;
			this.rdBtnLandscape.Name = "rdBtnLandscape";
			this.rdBtnLandscape.DrawIndicator = true;
			this.rdBtnLandscape.UseUnderline = true;
			this.rdBtnLandscape.Group = new global::GLib.SList (global::System.IntPtr.Zero);
			this.hboxModus.Add (this.rdBtnLandscape);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hboxModus [this.rdBtnLandscape]));
			w3.Position = 0;
			// Container child hboxModus.Gtk.Box+BoxChild
			this.rdBtnPortrait = new global::Gtk.RadioButton (global::Mono.Unix.Catalog.GetString ("Portrait"));
			this.rdBtnPortrait.CanFocus = true;
			this.rdBtnPortrait.Name = "rdBtnPortrait";
			this.rdBtnPortrait.DrawIndicator = true;
			this.rdBtnPortrait.UseUnderline = true;
			this.rdBtnPortrait.Group = this.rdBtnLandscape.Group;
			this.hboxModus.Add (this.rdBtnPortrait);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hboxModus [this.rdBtnPortrait]));
			w4.Position = 1;
			this.AlignmentFrameModus.Add (this.hboxModus);
			this.frameModus.Add (this.AlignmentFrameModus);
			this.lbFrameModus = new global::Gtk.Label ();
			this.lbFrameModus.Name = "lbFrameModus";
			this.lbFrameModus.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Modus</b>");
			this.lbFrameModus.UseMarkup = true;
			this.frameModus.LabelWidget = this.lbFrameModus;
			this.vboxStitch.Add (this.frameModus);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vboxStitch [this.frameModus]));
			w7.Position = 0;
			w7.Expand = false;
			w7.Fill = false;
			// Container child vboxStitch.Gtk.Box+BoxChild
			this.frameImagePositions = new global::Gtk.Frame ();
			this.frameImagePositions.Name = "frameImagePositions";
			// Container child frameImagePositions.Gtk.Container+ContainerChild
			this.AlignmentImagePositions = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentImagePositions.Name = "AlignmentImagePositions";
			this.AlignmentImagePositions.LeftPadding = ((uint)(12));
			// Container child AlignmentImagePositions.Gtk.Container+ContainerChild
			this.tableImagePosition1 = new global::Gtk.Table (((uint)(3)), ((uint)(10)), false);
			this.tableImagePosition1.Name = "tableImagePosition1";
			this.tableImagePosition1.RowSpacing = ((uint)(6));
			this.tableImagePosition1.ColumnSpacing = ((uint)(6));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01BottomMinus = new global::Picturez.PicturezButton ();
			this.btn01BottomMinus.Name = "btn01BottomMinus";
			this.btn01BottomMinus.CheckReleaseState = false;
			this.btn01BottomMinus.BorderlineWidth = 3;
			this.btn01BottomMinus.ButtonHeight = 25;
			this.btn01BottomMinus.ButtonWidth = 25;
			this.btn01BottomMinus.Font = "Arial";
			this.btn01BottomMinus.Text = "-";
			this.btn01BottomMinus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01BottomMinus);
			global::Gtk.Table.TableChild w8 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01BottomMinus]));
			w8.TopAttach = ((uint)(2));
			w8.BottomAttach = ((uint)(3));
			w8.LeftAttach = ((uint)(3));
			w8.RightAttach = ((uint)(4));
			w8.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01BottomPlus = new global::Picturez.PicturezButton ();
			this.btn01BottomPlus.Name = "btn01BottomPlus";
			this.btn01BottomPlus.CheckReleaseState = false;
			this.btn01BottomPlus.BorderlineWidth = 3;
			this.btn01BottomPlus.ButtonHeight = 25;
			this.btn01BottomPlus.ButtonWidth = 25;
			this.btn01BottomPlus.Font = "Arial";
			this.btn01BottomPlus.Text = "+";
			this.btn01BottomPlus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01BottomPlus);
			global::Gtk.Table.TableChild w9 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01BottomPlus]));
			w9.TopAttach = ((uint)(2));
			w9.BottomAttach = ((uint)(3));
			w9.LeftAttach = ((uint)(2));
			w9.RightAttach = ((uint)(3));
			w9.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01LeftMinus = new global::Picturez.PicturezButton ();
			this.btn01LeftMinus.Name = "btn01LeftMinus";
			this.btn01LeftMinus.CheckReleaseState = false;
			this.btn01LeftMinus.BorderlineWidth = 3;
			this.btn01LeftMinus.ButtonHeight = 25;
			this.btn01LeftMinus.ButtonWidth = 25;
			this.btn01LeftMinus.Font = "Arial";
			this.btn01LeftMinus.Text = "-";
			this.btn01LeftMinus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01LeftMinus);
			global::Gtk.Table.TableChild w10 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01LeftMinus]));
			w10.TopAttach = ((uint)(1));
			w10.BottomAttach = ((uint)(2));
			w10.LeftAttach = ((uint)(1));
			w10.RightAttach = ((uint)(2));
			w10.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01LeftPlus = new global::Picturez.PicturezButton ();
			this.btn01LeftPlus.Name = "btn01LeftPlus";
			this.btn01LeftPlus.CheckReleaseState = false;
			this.btn01LeftPlus.BorderlineWidth = 3;
			this.btn01LeftPlus.ButtonHeight = 25;
			this.btn01LeftPlus.ButtonWidth = 25;
			this.btn01LeftPlus.Font = "Arial";
			this.btn01LeftPlus.Text = "+";
			this.btn01LeftPlus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01LeftPlus);
			global::Gtk.Table.TableChild w11 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01LeftPlus]));
			w11.TopAttach = ((uint)(1));
			w11.BottomAttach = ((uint)(2));
			w11.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01RightMinus = new global::Picturez.PicturezButton ();
			this.btn01RightMinus.Name = "btn01RightMinus";
			this.btn01RightMinus.CheckReleaseState = false;
			this.btn01RightMinus.BorderlineWidth = 3;
			this.btn01RightMinus.ButtonHeight = 25;
			this.btn01RightMinus.ButtonWidth = 25;
			this.btn01RightMinus.Font = "Arial";
			this.btn01RightMinus.Text = "-";
			this.btn01RightMinus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01RightMinus);
			global::Gtk.Table.TableChild w12 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01RightMinus]));
			w12.TopAttach = ((uint)(1));
			w12.BottomAttach = ((uint)(2));
			w12.LeftAttach = ((uint)(5));
			w12.RightAttach = ((uint)(6));
			w12.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01RightPlus = new global::Picturez.PicturezButton ();
			this.btn01RightPlus.Name = "btn01RightPlus";
			this.btn01RightPlus.CheckReleaseState = false;
			this.btn01RightPlus.BorderlineWidth = 3;
			this.btn01RightPlus.ButtonHeight = 25;
			this.btn01RightPlus.ButtonWidth = 25;
			this.btn01RightPlus.Font = "Arial";
			this.btn01RightPlus.Text = "+";
			this.btn01RightPlus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01RightPlus);
			global::Gtk.Table.TableChild w13 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01RightPlus]));
			w13.TopAttach = ((uint)(1));
			w13.BottomAttach = ((uint)(2));
			w13.LeftAttach = ((uint)(4));
			w13.RightAttach = ((uint)(5));
			w13.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01TopMinus = new global::Picturez.PicturezButton ();
			this.btn01TopMinus.Name = "btn01TopMinus";
			this.btn01TopMinus.CheckReleaseState = false;
			this.btn01TopMinus.BorderlineWidth = 3;
			this.btn01TopMinus.ButtonHeight = 25;
			this.btn01TopMinus.ButtonWidth = 25;
			this.btn01TopMinus.Font = "Arial";
			this.btn01TopMinus.Text = "-";
			this.btn01TopMinus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01TopMinus);
			global::Gtk.Table.TableChild w14 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01TopMinus]));
			w14.LeftAttach = ((uint)(3));
			w14.RightAttach = ((uint)(4));
			w14.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.btn01TopPlus = new global::Picturez.PicturezButton ();
			this.btn01TopPlus.Name = "btn01TopPlus";
			this.btn01TopPlus.CheckReleaseState = false;
			this.btn01TopPlus.BorderlineWidth = 3;
			this.btn01TopPlus.ButtonHeight = 25;
			this.btn01TopPlus.ButtonWidth = 25;
			this.btn01TopPlus.Font = "Arial";
			this.btn01TopPlus.Text = "+";
			this.btn01TopPlus.TextSize = 14;
			this.tableImagePosition1.Add (this.btn01TopPlus);
			global::Gtk.Table.TableChild w15 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.btn01TopPlus]));
			w15.LeftAttach = ((uint)(2));
			w15.RightAttach = ((uint)(3));
			w15.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.lb01Bottom = new global::Gtk.Label ();
			this.lb01Bottom.Name = "lb01Bottom";
			this.lb01Bottom.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition1.Add (this.lb01Bottom);
			global::Gtk.Table.TableChild w16 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.lb01Bottom]));
			w16.TopAttach = ((uint)(2));
			w16.BottomAttach = ((uint)(3));
			w16.LeftAttach = ((uint)(8));
			w16.RightAttach = ((uint)(9));
			w16.XOptions = ((global::Gtk.AttachOptions)(4));
			w16.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.lb01Left = new global::Gtk.Label ();
			this.lb01Left.Name = "lb01Left";
			this.lb01Left.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition1.Add (this.lb01Left);
			global::Gtk.Table.TableChild w17 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.lb01Left]));
			w17.TopAttach = ((uint)(1));
			w17.BottomAttach = ((uint)(2));
			w17.LeftAttach = ((uint)(7));
			w17.RightAttach = ((uint)(8));
			w17.XOptions = ((global::Gtk.AttachOptions)(4));
			w17.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.lb01Right = new global::Gtk.Label ();
			this.lb01Right.Name = "lb01Right";
			this.lb01Right.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition1.Add (this.lb01Right);
			global::Gtk.Table.TableChild w18 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.lb01Right]));
			w18.TopAttach = ((uint)(1));
			w18.BottomAttach = ((uint)(2));
			w18.LeftAttach = ((uint)(9));
			w18.RightAttach = ((uint)(10));
			w18.XOptions = ((global::Gtk.AttachOptions)(4));
			w18.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition1.Gtk.Table+TableChild
			this.lb01Top = new global::Gtk.Label ();
			this.lb01Top.Name = "lb01Top";
			this.lb01Top.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition1.Add (this.lb01Top);
			global::Gtk.Table.TableChild w19 = ((global::Gtk.Table.TableChild)(this.tableImagePosition1 [this.lb01Top]));
			w19.LeftAttach = ((uint)(8));
			w19.RightAttach = ((uint)(9));
			w19.XOptions = ((global::Gtk.AttachOptions)(4));
			w19.YOptions = ((global::Gtk.AttachOptions)(4));
			this.AlignmentImagePositions.Add (this.tableImagePosition1);
			this.frameImagePositions.Add (this.AlignmentImagePositions);
			this.lbFrameImagePositions = new global::Gtk.Label ();
			this.lbFrameImagePositions.Name = "lbFrameImagePositions";
			this.lbFrameImagePositions.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Image Positions</b>");
			this.lbFrameImagePositions.UseMarkup = true;
			this.frameImagePositions.LabelWidget = this.lbFrameImagePositions;
			this.vboxStitch.Add (this.frameImagePositions);
			global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.vboxStitch [this.frameImagePositions]));
			w22.Position = 1;
			w22.Expand = false;
			w22.Fill = false;
			// Container child vboxStitch.Gtk.Box+BoxChild
			this.frameImagePositions2 = new global::Gtk.Frame ();
			this.frameImagePositions2.Name = "frameImagePositions2";
			// Container child frameImagePositions2.Gtk.Container+ContainerChild
			this.AlignmentImagePositions1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.AlignmentImagePositions1.Name = "AlignmentImagePositions1";
			this.AlignmentImagePositions1.LeftPadding = ((uint)(12));
			// Container child AlignmentImagePositions1.Gtk.Container+ContainerChild
			this.tableImagePosition2 = new global::Gtk.Table (((uint)(3)), ((uint)(10)), false);
			this.tableImagePosition2.Name = "tableImagePosition2";
			this.tableImagePosition2.RowSpacing = ((uint)(6));
			this.tableImagePosition2.ColumnSpacing = ((uint)(6));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02BottomMinus = new global::Picturez.PicturezButton ();
			this.btn02BottomMinus.Name = "btn02BottomMinus";
			this.btn02BottomMinus.CheckReleaseState = false;
			this.btn02BottomMinus.BorderlineWidth = 3;
			this.btn02BottomMinus.ButtonHeight = 25;
			this.btn02BottomMinus.ButtonWidth = 25;
			this.btn02BottomMinus.Font = "Arial";
			this.btn02BottomMinus.Text = "-";
			this.btn02BottomMinus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02BottomMinus);
			global::Gtk.Table.TableChild w23 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02BottomMinus]));
			w23.TopAttach = ((uint)(2));
			w23.BottomAttach = ((uint)(3));
			w23.LeftAttach = ((uint)(3));
			w23.RightAttach = ((uint)(4));
			w23.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02BottomPlus = new global::Picturez.PicturezButton ();
			this.btn02BottomPlus.Name = "btn02BottomPlus";
			this.btn02BottomPlus.CheckReleaseState = false;
			this.btn02BottomPlus.BorderlineWidth = 3;
			this.btn02BottomPlus.ButtonHeight = 25;
			this.btn02BottomPlus.ButtonWidth = 25;
			this.btn02BottomPlus.Font = "Arial";
			this.btn02BottomPlus.Text = "+";
			this.btn02BottomPlus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02BottomPlus);
			global::Gtk.Table.TableChild w24 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02BottomPlus]));
			w24.TopAttach = ((uint)(2));
			w24.BottomAttach = ((uint)(3));
			w24.LeftAttach = ((uint)(2));
			w24.RightAttach = ((uint)(3));
			w24.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02LeftMinus = new global::Picturez.PicturezButton ();
			this.btn02LeftMinus.Name = "btn02LeftMinus";
			this.btn02LeftMinus.CheckReleaseState = false;
			this.btn02LeftMinus.BorderlineWidth = 3;
			this.btn02LeftMinus.ButtonHeight = 25;
			this.btn02LeftMinus.ButtonWidth = 25;
			this.btn02LeftMinus.Font = "Arial";
			this.btn02LeftMinus.Text = "-";
			this.btn02LeftMinus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02LeftMinus);
			global::Gtk.Table.TableChild w25 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02LeftMinus]));
			w25.TopAttach = ((uint)(1));
			w25.BottomAttach = ((uint)(2));
			w25.LeftAttach = ((uint)(1));
			w25.RightAttach = ((uint)(2));
			w25.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02LeftPlus = new global::Picturez.PicturezButton ();
			this.btn02LeftPlus.Name = "btn02LeftPlus";
			this.btn02LeftPlus.CheckReleaseState = false;
			this.btn02LeftPlus.BorderlineWidth = 3;
			this.btn02LeftPlus.ButtonHeight = 25;
			this.btn02LeftPlus.ButtonWidth = 25;
			this.btn02LeftPlus.Font = "Arial";
			this.btn02LeftPlus.Text = "+";
			this.btn02LeftPlus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02LeftPlus);
			global::Gtk.Table.TableChild w26 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02LeftPlus]));
			w26.TopAttach = ((uint)(1));
			w26.BottomAttach = ((uint)(2));
			w26.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02RightMinus = new global::Picturez.PicturezButton ();
			this.btn02RightMinus.Name = "btn02RightMinus";
			this.btn02RightMinus.CheckReleaseState = false;
			this.btn02RightMinus.BorderlineWidth = 3;
			this.btn02RightMinus.ButtonHeight = 25;
			this.btn02RightMinus.ButtonWidth = 25;
			this.btn02RightMinus.Font = "Arial";
			this.btn02RightMinus.Text = "-";
			this.btn02RightMinus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02RightMinus);
			global::Gtk.Table.TableChild w27 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02RightMinus]));
			w27.TopAttach = ((uint)(1));
			w27.BottomAttach = ((uint)(2));
			w27.LeftAttach = ((uint)(5));
			w27.RightAttach = ((uint)(6));
			w27.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02RightPlus = new global::Picturez.PicturezButton ();
			this.btn02RightPlus.Name = "btn02RightPlus";
			this.btn02RightPlus.CheckReleaseState = false;
			this.btn02RightPlus.BorderlineWidth = 3;
			this.btn02RightPlus.ButtonHeight = 25;
			this.btn02RightPlus.ButtonWidth = 25;
			this.btn02RightPlus.Font = "Arial";
			this.btn02RightPlus.Text = "+";
			this.btn02RightPlus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02RightPlus);
			global::Gtk.Table.TableChild w28 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02RightPlus]));
			w28.TopAttach = ((uint)(1));
			w28.BottomAttach = ((uint)(2));
			w28.LeftAttach = ((uint)(4));
			w28.RightAttach = ((uint)(5));
			w28.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02TopMinus = new global::Picturez.PicturezButton ();
			this.btn02TopMinus.Name = "btn02TopMinus";
			this.btn02TopMinus.CheckReleaseState = false;
			this.btn02TopMinus.BorderlineWidth = 3;
			this.btn02TopMinus.ButtonHeight = 25;
			this.btn02TopMinus.ButtonWidth = 25;
			this.btn02TopMinus.Font = "Arial";
			this.btn02TopMinus.Text = "-";
			this.btn02TopMinus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02TopMinus);
			global::Gtk.Table.TableChild w29 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02TopMinus]));
			w29.LeftAttach = ((uint)(3));
			w29.RightAttach = ((uint)(4));
			w29.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.btn02TopPlus = new global::Picturez.PicturezButton ();
			this.btn02TopPlus.Name = "btn02TopPlus";
			this.btn02TopPlus.CheckReleaseState = false;
			this.btn02TopPlus.BorderlineWidth = 3;
			this.btn02TopPlus.ButtonHeight = 25;
			this.btn02TopPlus.ButtonWidth = 25;
			this.btn02TopPlus.Font = "Arial";
			this.btn02TopPlus.Text = "+";
			this.btn02TopPlus.TextSize = 14;
			this.tableImagePosition2.Add (this.btn02TopPlus);
			global::Gtk.Table.TableChild w30 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.btn02TopPlus]));
			w30.LeftAttach = ((uint)(2));
			w30.RightAttach = ((uint)(3));
			w30.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.lb02Bottom = new global::Gtk.Label ();
			this.lb02Bottom.Name = "lb02Bottom";
			this.lb02Bottom.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition2.Add (this.lb02Bottom);
			global::Gtk.Table.TableChild w31 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.lb02Bottom]));
			w31.TopAttach = ((uint)(2));
			w31.BottomAttach = ((uint)(3));
			w31.LeftAttach = ((uint)(8));
			w31.RightAttach = ((uint)(9));
			w31.XOptions = ((global::Gtk.AttachOptions)(4));
			w31.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.lb02Left = new global::Gtk.Label ();
			this.lb02Left.Name = "lb02Left";
			this.lb02Left.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition2.Add (this.lb02Left);
			global::Gtk.Table.TableChild w32 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.lb02Left]));
			w32.TopAttach = ((uint)(1));
			w32.BottomAttach = ((uint)(2));
			w32.LeftAttach = ((uint)(7));
			w32.RightAttach = ((uint)(8));
			w32.XOptions = ((global::Gtk.AttachOptions)(4));
			w32.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.lb02Right = new global::Gtk.Label ();
			this.lb02Right.Name = "lb02Right";
			this.lb02Right.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition2.Add (this.lb02Right);
			global::Gtk.Table.TableChild w33 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.lb02Right]));
			w33.TopAttach = ((uint)(1));
			w33.BottomAttach = ((uint)(2));
			w33.LeftAttach = ((uint)(9));
			w33.RightAttach = ((uint)(10));
			w33.XOptions = ((global::Gtk.AttachOptions)(4));
			w33.YOptions = ((global::Gtk.AttachOptions)(4));
			// Container child tableImagePosition2.Gtk.Table+TableChild
			this.lb02Top = new global::Gtk.Label ();
			this.lb02Top.Name = "lb02Top";
			this.lb02Top.LabelProp = global::Mono.Unix.Catalog.GetString ("0");
			this.tableImagePosition2.Add (this.lb02Top);
			global::Gtk.Table.TableChild w34 = ((global::Gtk.Table.TableChild)(this.tableImagePosition2 [this.lb02Top]));
			w34.LeftAttach = ((uint)(8));
			w34.RightAttach = ((uint)(9));
			w34.XOptions = ((global::Gtk.AttachOptions)(4));
			w34.YOptions = ((global::Gtk.AttachOptions)(4));
			this.AlignmentImagePositions1.Add (this.tableImagePosition2);
			this.frameImagePositions2.Add (this.AlignmentImagePositions1);
			this.lbFrameImagePositions2 = new global::Gtk.Label ();
			this.lbFrameImagePositions2.Name = "lbFrameImagePositions2";
			this.lbFrameImagePositions2.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Image Positions 02</b>");
			this.lbFrameImagePositions2.UseMarkup = true;
			this.frameImagePositions2.LabelWidget = this.lbFrameImagePositions2;
			this.vboxStitch.Add (this.frameImagePositions2);
			global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vboxStitch [this.frameImagePositions2]));
			w37.Position = 2;
			w37.Expand = false;
			w37.Fill = false;
			this.AlignmentFrameStitch.Add (this.vboxStitch);
			this.frameStitch.Add (this.AlignmentFrameStitch);
			this.lbFrameStitch = new global::Gtk.Label ();
			this.lbFrameStitch.Name = "lbFrameStitch";
			this.lbFrameStitch.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Stitch</b>");
			this.lbFrameStitch.UseMarkup = true;
			this.frameStitch.LabelWidget = this.lbFrameStitch;
			this.vbox1.Add (this.frameStitch);
			global::Gtk.Box.BoxChild w40 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frameStitch]));
			w40.Position = 0;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frameImageResolution = new global::Gtk.Frame ();
			this.frameImageResolution.Name = "frameImageResolution";
			// Container child frameImageResolution.Gtk.Container+ContainerChild
			this.GtkAlignment2 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment2.Name = "GtkAlignment2";
			this.GtkAlignment2.LeftPadding = ((uint)(12));
			// Container child GtkAlignment2.Gtk.Container+ContainerChild
			this.lbImageResolution = new global::Gtk.Label ();
			this.lbImageResolution.Name = "lbImageResolution";
			this.lbImageResolution.LabelProp = global::Mono.Unix.Catalog.GetString ("0 x 0");
			this.GtkAlignment2.Add (this.lbImageResolution);
			this.frameImageResolution.Add (this.GtkAlignment2);
			this.lbFrameImageResolution = new global::Gtk.Label ();
			this.lbFrameImageResolution.Name = "lbFrameImageResolution";
			this.lbFrameImageResolution.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Image Resolution</b>");
			this.lbFrameImageResolution.UseMarkup = true;
			this.frameImageResolution.LabelWidget = this.lbFrameImageResolution;
			this.vbox1.Add (this.frameImageResolution);
			global::Gtk.Box.BoxChild w43 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frameImageResolution]));
			w43.Position = 1;
			w43.Expand = false;
			w43.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.frameCursorPos = new global::Gtk.Frame ();
			this.frameCursorPos.Name = "frameCursorPos";
			// Container child frameCursorPos.Gtk.Container+ContainerChild
			this.GtkAlignment1 = new global::Gtk.Alignment (0F, 0F, 1F, 1F);
			this.GtkAlignment1.Name = "GtkAlignment1";
			this.GtkAlignment1.LeftPadding = ((uint)(12));
			// Container child GtkAlignment1.Gtk.Container+ContainerChild
			this.lbCursorPos = new global::Gtk.Label ();
			this.lbCursorPos.Name = "lbCursorPos";
			this.lbCursorPos.LabelProp = global::Mono.Unix.Catalog.GetString ("0 x 0");
			this.GtkAlignment1.Add (this.lbCursorPos);
			this.frameCursorPos.Add (this.GtkAlignment1);
			this.lbFrameCursorPos = new global::Gtk.Label ();
			this.lbFrameCursorPos.Name = "lbFrameCursorPos";
			this.lbFrameCursorPos.LabelProp = global::Mono.Unix.Catalog.GetString ("<b>Cursor position</b>");
			this.lbFrameCursorPos.UseMarkup = true;
			this.frameCursorPos.LabelWidget = this.lbFrameCursorPos;
			this.vbox1.Add (this.frameCursorPos);
			global::Gtk.Box.BoxChild w46 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.frameCursorPos]));
			w46.Position = 2;
			w46.Expand = false;
			w46.Fill = false;
			// Container child vbox1.Gtk.Box+BoxChild
			this.btnOk = new global::Picturez.PicturezButton ();
			this.btnOk.Name = "btnOk";
			this.btnOk.CheckReleaseState = false;
			this.btnOk.BorderlineWidth = 3;
			this.btnOk.ButtonHeight = 35;
			this.btnOk.ButtonWidth = 0;
			this.btnOk.Font = "Arial";
			this.btnOk.Text = "OK";
			this.btnOk.TextSize = 14;
			this.vbox1.Add (this.btnOk);
			global::Gtk.Box.BoxChild w47 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.btnOk]));
			w47.Position = 3;
			w47.Expand = false;
			w47.Fill = false;
			this.hbox1.Add (this.vbox1);
			global::Gtk.Box.BoxChild w48 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.vbox1]));
			w48.Position = 1;
			w48.Padding = ((uint)(5));
			this.vboxA.Add (this.hbox1);
			global::Gtk.Box.BoxChild w49 = ((global::Gtk.Box.BoxChild)(this.vboxA [this.hbox1]));
			w49.Position = 1;
			this.Add (this.vboxA);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 524;
			this.DefaultHeight = 508;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
			this.btn01TopPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01TopPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01TopMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01TopMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01RightPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01RightPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01RightMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01RightMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01LeftPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01LeftPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01LeftMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01LeftMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01BottomPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01BottomPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn01BottomMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn01BottomMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02TopPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02TopPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02TopMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02TopMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02RightPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02RightPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02RightMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02RightMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02LeftPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02LeftPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02LeftMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02LeftMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02BottomPlus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02BottomPlus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btn02BottomMinus.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnReleaseEvent);
			this.btn02BottomMinus.ButtonPressEvent += new global::Gtk.ButtonPressEventHandler (this.OnBtnPressEvent);
			this.btnOk.ButtonReleaseEvent += new global::Gtk.ButtonReleaseEventHandler (this.OnBtnOkButtonReleaseEvent);
		}
	}
}
