//
// ImageTag.cs: This abstract class extends the Tag class by basic Image
// properties.
//
// Author:
//   Mike Gemuende (mike@gemuende.de)
//   Paul Lange (palango@gmx.de)
//
// Copyright (C) 2009 Mike Gemuende
//
// This library is free software; you can redistribute it and/or modify
// it  under the terms of the GNU Lesser General Public License version
// 2.1 as published by the Free Software Foundation.
//
// This library is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
// USA
//

using System;

namespace TagLib.Image
{

	/// <summary>
	///    A class to abstract the image tags. It extends the <see cref="Tag"/>
	///    class and adds some image specific propties.
	/// </summary>
	public abstract class ImageTag : Tag
	{

#region Public Properties

		/// <summary>
		///    Gets or sets the keywords for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="string[]" /> containing the keywords of the
		///    current instace.
		/// </value>
		public virtual string[] Keywords {
			get { return new string [] {}; }
			set {}
		}

		/// <summary>
		///    Gets or sets the rating for the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> containing the rating of the
		///    current instace.
		/// </value>
		public virtual uint? Rating {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the time when the image, the current instance
		///    belongs to, was taken.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the time the image was taken.
		/// </value>
		public virtual DateTime? DateTime {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the orientation of the image described
		///    by the current instance.
		/// </summary>
		/// <value>
		///    A <see cref="TagLib.Image.ImageOrientation" /> containing the orientation of the
		///    image
		/// </value>
		public virtual ImageOrientation Orientation {
			get { return ImageOrientation.None; }
			set {}
		}

		/// <summary>
		///    Gets or sets the software the image, the current instance
		///    belongs to, was created with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> containing the name of the
		///    software the current instace was created with.
		/// </value>
		public virtual string Software {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the latitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the latitude ranging from -90.0
		///    to +90.0 degrees.
		/// </value>
		public virtual double? Latitude {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the longitude of the GPS coordinate the current
		///    image was taken.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the longitude ranging from -180.0
		///    to +180.0 degrees.
		/// </value>
		public virtual double? Longitude {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the altitude of the GPS coordinate the current
		///    image was taken. The unit is meter.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the altitude. A positive value
		///    is above sea level, a negative one below sea level. The unit is meter.
		/// </value>
		public virtual double? Altitude {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the exposure time the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the exposure time in seconds.
		/// </value>
		public virtual double? ExposureTime {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the FNumber the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the FNumber.
		/// </value>
		public virtual double? FNumber {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the ISO speed the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the ISO speed as defined in ISO 12232.
		/// </value>
		public virtual uint? ISOSpeedRatings {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the focal length the image, the current instance belongs
		///    to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the focal length in millimeters.
		/// </value>
		public virtual double? FocalLength {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the focal length the image, the current instance belongs
		///    to, was taken with, assuming a 35mm film camera.
		/// </summary>
		/// <value>
		///    A <see cref="System.Nullable"/> with the focal length in 35mm equivalent in millimeters.
		/// </value>
		public virtual uint? FocalLengthIn35mmFilm {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the manufacture of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the manufacture name.
		/// </value>
		public virtual string Make {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets the model name of the recording equipment the image, the
		///    current instance belongs to, was taken with.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the model name.
		/// </value>
		public virtual string Model {
			get { return null; }
			set {}
		}

		/// <summary>
		///    Gets or sets the creator of the image.
		/// </summary>
		/// <value>
		///    A <see cref="string" /> with the name of the creator.
		/// </value>
		public virtual string Creator {
			get { return null; }
			set {}
		}

#endregion

		public override void CopyTo (Tag target, bool overwrite)
		{
			base.CopyTo (target, overwrite);
			ImageTag t = target as ImageTag;

			if (overwrite || IsNullOrLikeEmpty (t.Altitude))
				t.Altitude = Altitude;

			if (overwrite || IsNullOrLikeEmpty (t.Creator))
				t.Creator = Creator;


			if (overwrite || IsNullOrLikeEmpty (t.DateTime))
				t.DateTime = DateTime;

			if (overwrite || IsNullOrLikeEmpty (t.ExposureTime))
				t.ExposureTime = ExposureTime;

			if (overwrite || IsNullOrLikeEmpty (t.FNumber))
				t.FNumber = FNumber;

			if (overwrite || IsNullOrLikeEmpty (t.FocalLength))
				t.FocalLength = FocalLength;

			if (overwrite || IsNullOrLikeEmpty (t.FocalLengthIn35mmFilm))
				t.FocalLengthIn35mmFilm = FocalLengthIn35mmFilm;

			if (overwrite || IsNullOrLikeEmpty (t.ISOSpeedRatings))
				t.ISOSpeedRatings = ISOSpeedRatings;

			if (overwrite || IsNullOrLikeEmpty (t.Keywords))
				t.Keywords = Keywords;

			if (overwrite || IsNullOrLikeEmpty (t.Latitude))
				t.Latitude = Latitude;

			if (overwrite || IsNullOrLikeEmpty (t.Longitude))
				t.Longitude = Longitude;

			if (overwrite || IsNullOrLikeEmpty (t.Make))
				t.Make = Make;

			if (overwrite || IsNullOrLikeEmpty (t.Model))
				t.Model = Model;

			if (overwrite || t.Orientation == ImageOrientation.None)
				t.Orientation = Orientation;

			if (overwrite || IsNullOrLikeEmpty (t.Rating))
				t.Rating = Rating;

			if (overwrite || IsNullOrLikeEmpty (t.Software))
				t.Software = Software;
		}


//		/// <summary>
//		/// Copies properties of passed tag. If overwrite is true, 
//		/// the copy process will be forced. Otherwise only copy, 
//		/// if own value is not set.
//		/// </summary>
//		public virtual void PIC_ENHANCE_CopyFrom(ImageTag t, bool overwrite)
//		{
//			if ((!IsNullOrLikeEmpty (Altitude) && !IsNullOrLikeEmpty (t.Altitude) && overwrite) || IsNullOrLikeEmpty(Altitude))
//				this.Altitude = t.Altitude;
//
//			if ((!IsNullOrLikeEmpty (Creator) && !IsNullOrLikeEmpty (t.Creator) && overwrite) || IsNullOrLikeEmpty (Creator))
//				this.Creator = t.Creator;
//
//			if ((!IsNullOrLikeEmpty (DateTime) && !IsNullOrLikeEmpty (t.DateTime) && overwrite) || IsNullOrLikeEmpty (DateTime))
//				this.DateTime = t.DateTime;
//
//			if ((!IsNullOrLikeEmpty (ExposureTime) && !IsNullOrLikeEmpty (t.ExposureTime) && overwrite) || IsNullOrLikeEmpty (ExposureTime))
//				this.ExposureTime = t.ExposureTime;
//
//			if ((!IsNullOrLikeEmpty (FNumber) && !IsNullOrLikeEmpty (t.FNumber) && overwrite) || IsNullOrLikeEmpty (FNumber))
//				this.FNumber = t.FNumber;
//
//			if ((!IsNullOrLikeEmpty (FocalLength) && !IsNullOrLikeEmpty (t.FocalLength) && overwrite) || IsNullOrLikeEmpty (FocalLength))
//				this.FocalLength = t.FocalLength;
//
//			if ((!IsNullOrLikeEmpty (FocalLengthIn35mmFilm) && !IsNullOrLikeEmpty (t.FocalLengthIn35mmFilm) && overwrite) || IsNullOrLikeEmpty (FocalLengthIn35mmFilm))
//				this.FocalLengthIn35mmFilm = t.FocalLengthIn35mmFilm;
//
//			if ((!IsNullOrLikeEmpty (ISOSpeedRatings) && !IsNullOrLikeEmpty (t.ISOSpeedRatings) && overwrite) || IsNullOrLikeEmpty (ISOSpeedRatings))
//				this.ISOSpeedRatings = t.ISOSpeedRatings;
//
//			if ((!IsNullOrLikeEmpty (Keywords) && !IsNullOrLikeEmpty (t.Keywords) && overwrite) || IsNullOrLikeEmpty (Keywords))
//				this.Keywords = t.Keywords;
//
//			if ((!IsNullOrLikeEmpty (Latitude) && !IsNullOrLikeEmpty (t.Latitude) && overwrite) || IsNullOrLikeEmpty (Latitude))
//				this.Latitude = t.Latitude;
//
//			if ((!IsNullOrLikeEmpty (Longitude) && !IsNullOrLikeEmpty (t.Longitude) && overwrite) || IsNullOrLikeEmpty (Longitude))
//				this.Longitude = t.Longitude;
//
//			if ((!IsNullOrLikeEmpty (Make) && !IsNullOrLikeEmpty (t.Make) && overwrite) || IsNullOrLikeEmpty (Make))
//				this.Make = t.Make;
//
//			if ((!IsNullOrLikeEmpty (Model) && !IsNullOrLikeEmpty (t.Model) && overwrite) || IsNullOrLikeEmpty (Model))
//				this.Model = t.Model;
//
//			if ((Orientation != ImageOrientation.None && t.Orientation != ImageOrientation.None && overwrite) || Orientation == ImageOrientation.None)
//				this.Orientation = t.Orientation;
//
//			if ((!IsNullOrLikeEmpty (Rating) && !IsNullOrLikeEmpty (t.Rating) && overwrite) || IsNullOrLikeEmpty (Creator))
//			this.Rating = t.Rating;
//
//			if ((!IsNullOrLikeEmpty (Software) && !IsNullOrLikeEmpty (t.Software) && overwrite) || IsNullOrLikeEmpty (Software))
//				this.Software = t.Software;
//		}

		protected static bool IsNullOrLikeEmpty (double? value)
		{
			return value == null;
		}

		protected static bool IsNullOrLikeEmpty (DateTime? value)
		{
			return value == null;
		}
	}
}
