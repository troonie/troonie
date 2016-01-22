using System;
// using System.Drawing;
using NetColor = System.Drawing.Color;
using GdkColor = Gdk.Color;
using CairoColor = Cairo.Color;

namespace Picturez
{
	public class ColorConverter
	{
		#region PressedInButton colors

		public abstract class Btn
		{
			public CairoColor Top { get; protected set; }
			public CairoColor Middle { get; protected set; }
			public CairoColor Down { get; protected set; }
			public CairoColor Font { get; protected set; }
			public CairoColor Border { get; set; }

//			protected Btn()
//			{
//				Border = ColorConverter.Instance.C_GRID;
//			}
		}

		public class BtnNormal : Btn
		{
			private static BtnNormal i;
			public static BtnNormal I
			{
				get
				{
					if (i == null)
						i = new BtnNormal ();

					return i;
				}
			}

			public BtnNormal() : base()
			{
				Top = new CairoColor (226 / 255.0, 241 / 255.0, 250 / 255.0);	
				Middle = new CairoColor (123 / 255.0, 192 / 255.0, 232 / 255.0);
				Down = new CairoColor (170 / 255.0, 244 / 255.0, 252 / 255.0);
				Font = new CairoColor (68 / 255.0, 65 / 255.0, 174 / 255.0);
				Border = ColorConverter.Instance.C_GRID;
			}
		}

//		public class BtnMouseover : BtnNormal
//		{
//			private static BtnMouseover i;
//			public static new BtnMouseover I
//			{
//				get
//				{
//					if (i == null)
//						i = new BtnMouseover ();
//
//					return i;
//				}
//			}
//
//			public BtnMouseover() : base()
//			{
//				Border = ColorConverter.Instance.Cairo_Orange;
//			}
//		}

		public class BtnPressedin : Btn
		{
			private static BtnPressedin i;
			public static BtnPressedin I
			{
				get
				{
					if (i == null)
						i = new BtnPressedin ();

					return i;
				}
			}

			public BtnPressedin() : base()
			{
				Top = new CairoColor (255 / 255.0, 246 / 255.0, 216 / 255.0);
				Middle = new CairoColor (255 / 255.0, 213 / 255.0, 77 / 255.0);
				Down = new CairoColor (255 / 255.0, 233 / 255.0, 155 / 255.0);
				Font = new CairoColor (21 / 255.0, 66 / 255.0, 139 / 255.0);
				Border = new CairoColor (220 / 255.0, 205 / 255.0, 153 / 255.0);
			}
		}

		public class BtnPressed : Btn
		{
			private static BtnPressed i;
			public static BtnPressed I
			{
				get
				{
					if (i == null)
						i = new BtnPressed ();

					return i;
				}
			}

			public BtnPressed() : base()
			{
				Top = new CairoColor (245 / 255.0, 191 / 255.0, 135 / 255.0);
				Middle = new CairoColor (235 / 255.0, 122 / 255.0, 5 / 255.0);
				Down = new CairoColor (249 / 255.0, 176 / 255.0, 81 / 255.0);
				Font = new CairoColor (114 / 255.0, 65 / 255.0, 107 / 255.0);
				Border = new CairoColor (154 / 255.0, 129 / 255.0, 89 / 255.0);
			}
		}

		#endregion PressedInButton colors

		private static ColorConverter instance;
		public static ColorConverter Instance
		{
			get
			{
				if (instance == null)
					instance = new ColorConverter ();

				return instance;
			}
		}

		public GdkColor GRID { get; private set; }
		public GdkColor FONT { get; private set; }
		public GdkColor AliceBlue { get; private set; }
		public GdkColor White { get; private set; }
		public GdkColor Red { get; private set; }
		public GdkColor Green { get; private set; }
		public GdkColor Blue { get; private set; }

		private CairoColor Cairo_AliceBlue { get; set; }
		public CairoColor Cairo_White { get; private set; }
		public CairoColor Cairo_Red { get; private set; }
		public CairoColor Cairo_Black { get; private set; }
		public CairoColor Cairo_Yellow { get; private set; }
		public CairoColor Cairo_Blue { get; private set; }

		public CairoColor Cairo_Orange { get; private set; }
		public CairoColor Cairo_Magenta { get; private set; }
		public CairoColor Cairo_Green { get; private set; }
		public CairoColor Cairo_BlueGreen { get; private set; }

		public CairoColor C_GRID { get; private set; }

		public ColorConverter()
		{
			GRID = new GdkColor (191, 219, 255);
			FONT = new GdkColor (68, 65, 174);
			AliceBlue = new Gdk.Color (240, 248, 255);
			White = new Gdk.Color (255, 255, 255);
			Red = new Gdk.Color (255, 0, 0);
			Green = new Gdk.Color (0, 255, 0);
			Blue = new Gdk.Color (0, 0, 255);

			Cairo_AliceBlue = new CairoColor (240 / 255.0, 248 / 255.0, 1);
			Cairo_White = new CairoColor (1, 1, 1);
			Cairo_Red = new CairoColor (1, 0, 0);
			Cairo_Black = new CairoColor (0, 0, 0);
			Cairo_Yellow = new CairoColor (1, 1, 0);
			Cairo_Blue = new CairoColor (0, 0, 1);
			Cairo_Orange = new CairoColor (1, 0.5, 0);
			Cairo_Magenta = new CairoColor (1, 0, 1);
			Cairo_Green = new CairoColor (0, 1, 0);
			Cairo_BlueGreen = new CairoColor (0, 1, 1);

			C_GRID = new CairoColor (191 / 255.0, 219 / 255.0, 255 / 255.0);
		}

		public NetColor ToDotNetColor(GdkColor c)
		{
			byte r = (byte) Math.Round((float)c.Red / ushort.MaxValue * 255);
			byte g = (byte) Math.Round((float)c.Green / ushort.MaxValue * 255);
			byte b = (byte) Math.Round((float)c.Blue / ushort.MaxValue * 255);

			NetColor nc = NetColor.FromArgb (r, g, b);
			return nc;
		}

		public void ToDotNetColor(GdkColor c, out byte red, out byte green, out byte blue)
		{
			red = (byte) Math.Round((float)c.Red / ushort.MaxValue * 255);
			green = (byte) Math.Round((float)c.Green / ushort.MaxValue * 255);
			blue = (byte) Math.Round((float)c.Blue / ushort.MaxValue * 255);
		}

		public CairoColor ToCairoColor (GdkColor color)
		{
			return new CairoColor ((double)color.Red / ushort.MaxValue,
				(double)color.Green / ushort.MaxValue, (double)color.Blue /
				ushort.MaxValue);
		}


		public GdkColor ToGdkColor (CairoColor color)
		{
			Gdk.Color c = new Gdk.Color ();
			c.Blue = (ushort)(color.B * ushort.MaxValue);
			c.Red = (ushort)(color.R * ushort.MaxValue);
			c.Green = (ushort)(color.G * ushort.MaxValue);

			return c;
		} 
	}
}

