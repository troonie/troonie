using System;
using System.IO;
using System.Xml.Serialization;

namespace Troonie_Lib
{    
    public class ConfigEdit
    {
		private static string editXmlFile = Constants.I.EXEPATH + "edit.xml"; 

        #region public properties 

        /// <summary>The bottom value. </summary>
        public int Bottom { get; set; }
        /// <summary>The left value. </summary>
        public int Left { get; set; }
//        /// <summary>The name of the configuration.</summary>
//        [XmlAttribute("Name", DataType = "string")]
//        public string Name { get; set; }
        /// <summary>The right value. </summary>
        public int Right { get; set; }
        /// <summary>The rotation value. </summary>
        public int Rotation { get; set; }
        /// <summary>The top value. </summary>
        public int Top { get; set; }

        #endregion public properties
		
        public ConfigEdit()
        {
//            Name = "";
        }

		public static ConfigEdit Load()
		{
			if (!File.Exists (editXmlFile)) {
				Save(new ConfigEdit());
			}

			XmlSerializer serializer = new XmlSerializer(typeof(ConfigEdit));
			StreamReader sr = new StreamReader(editXmlFile);
			ConfigEdit c = (ConfigEdit)serializer.Deserialize(sr);

			sr.Close();
			return c;
		}

		public static void Save(ConfigEdit c)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ConfigEdit));
			FileStream fs = new FileStream(editXmlFile, FileMode.Create); 
			serializer.Serialize(fs, c);
			fs.Close();
		}

	}
}
