using System;
using System.IO;
using Troonie_Lib;
using System.Linq;
using System.Collections.Generic;

namespace Troonie
{	
	public partial class ViewerWidget
	{
		private void AppendIdAndCompressionByRating()
		{		
			string tmp = "";
			try {	
				int nr = 0;

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel pib in pressedInVIPs) {
					tmp = pib.RelativeImageName;
					string creatorText = "";

					// check whether file is image or video
					FileInfo info = new FileInfo (pib.OriginalImageFullName);
					string ext = info.Extension.ToLower ();
					long origLength = info.Length; 
					uint rating;
					bool isVideo = false;
					//				DateTime? dt = null;
					TagsData td;
					string f = pib.RelativeImageName, fullf = pib.OriginalImageFullName;

					if (ext.Length != 0 && Constants.Extensions.Any (x => x.Value.Item1 == ext || x.Value.Item2 == ext)) {						
						td = ImageTagHelper.GetTagsData (pib.OriginalImageFullName);
						rating = td.Rating == null ? 0 : td.Rating.Value;

					} else {
						td = VideoTagHelper.GetTagsData(pib.OriginalImageFullName);
						rating = td.TrackCount;
						isVideo = true;

						//						VideoTagHelper.SetDateAndRatingInVideoTag (pib.OriginalImageFullName, 5);
						//						string f = pib.RelativeImageName, fullf = pib.OriginalImageFullName;
						ConvertWidget.InsertIdentifierAtBegin(ref f, ref fullf, "V-");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
					}

					long limitInBytes = Math.Max (rating * 1050000, 350000);
					int biggestLength;

					switch (rating) 
					{
					case 0:			
						break;
					case 1:
						ConvertWidget.AppendIdentifier (ref f, ref fullf, "_+");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 2: 
						ConvertWidget.AppendIdentifier (ref f, ref fullf, "_++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 3: 
						ConvertWidget.AppendIdentifier (ref f, ref fullf, "_+++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 4: 
						ConvertWidget.AppendIdentifier (ref f, ref fullf, "_++++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						break;
					case 5: 
						ConvertWidget.AppendIdentifier (ref f, ref fullf, "_+++++");
						SetTextAndFulltextAndRedrawVip(pib, f, fullf);
						limitInBytes = long.MaxValue; // avoid any jpg compression
						break;
					}

					if (!isVideo && (Constants.Extensions[TroonieImageFormat.JPEG24].Item1 == ext || 
						Constants.Extensions[TroonieImageFormat.JPEG24].Item2 == ext)) {
						byte jpqQuality = 95;
						biggestLength = 1800 + 1200 * (int)rating;
						if (origLength > limitInBytes &&
							TroonieBitmap.GetBiggestLength (pib.OriginalImageFullName) > biggestLength) 
						{
							ConvertWidget.ReduceImageSize (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, biggestLength, jpqQuality);
						}

						ConvertWidget.ConvertByRating (pib.RelativeImageName, pib.OriginalImageFullName, ref creatorText, limitInBytes, jpqQuality);
					}

					nr++;
				}
			}
			catch (Exception e)
			{
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [153];
				pseudo.Label1 = "Something went wrong by 'AppendIdAndCompressionByRating'.";
				pseudo.Label2 = "Stoppage at image '" + tmp + "'. Exception message: " + Constants.N + e.Message;
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.Show ();
			}
		}

		private static void SetTextAndFulltextAndRedrawVip(ViewerImagePanel vip, string filename, string fullfilename)
		{
			vip.RelativeImageName = filename; 
			vip.OriginalImageFullName = fullfilename;
			vip.QueueDraw ();
		}

		private void RenameByCreationDate()
		{
			string tmp = "";
			try{

				List<ViewerImagePanel>pressedInVIPs = GetPressedInVIPs();

				foreach (ViewerImagePanel vip in pressedInVIPs) {
					tmp = vip.RelativeImageName;
					string f = vip.RelativeImageName, fullf = vip.OriginalImageFullName;
					DateTime? dt = null;
					ImageTagHelper.GetDateTime (fullf, out dt);
					if (dt.HasValue) {
						ConvertWidget.RenameFileByDate (ref f, ref fullf, dt.Value);
						SetTextAndFulltextAndRedrawVip(vip, f, fullf);
					}
				}					
			}
			catch (Exception e)
			{
				OkCancelDialog pseudo = new OkCancelDialog (true);
				pseudo.Title = Language.I.L [153];
				pseudo.Label1 = "Something went wrong by 'RenameByCreationDate'.";
				pseudo.Label2 = "Stoppage at image '" + tmp + "'. Exception message: " + Constants.N + e.Message;
				pseudo.OkButtontext = Language.I.L [16];
				pseudo.Show ();
			}
		}
	}
}

