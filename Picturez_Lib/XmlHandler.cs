using System;
using System.IO;
using System.Xml.Serialization;

namespace Picturez_Lib
{
	public class XmlHandler
	{
		/// <summary>
		/// The xml file name, where the configurations are saved.</summary>
		private string configsXmlFile;
		/// <summary>
		/// The xml file name, where the fastcuts are saved.</summary>
		private string fastcutXmlFile;  

		private static XmlHandler instance;
		public static XmlHandler I
		{
			get
			{
				if (instance == null)
					instance = new XmlHandler ();

				return instance;
			}
		}

		public XmlHandler ()
		{
			configsXmlFile = Constants.I.EXEPATH + "configurations.xml"; 
			fastcutXmlFile = Constants.I.EXEPATH + "fastcuts.xml";			
		}

		/// <summary> Creates the Configurations and fastcuts Xml file. </summary>
		public void CreateXmlFiles()
		{
			if (!File.Exists (configsXmlFile)) {
				SaveXml(new Configurations());
			} 

			if (!File.Exists (fastcutXmlFile)) {
				SaveXml(new Fastcuts());
			}					
		}

		/// <summary>
		/// Loads the specified <paramref name="type"/> in a XML file.
		/// </summary>
		/// <param name="type">The type to load.</param>
		public IXmlType LoadXml(XmlTypes type)
		{
			StreamReader sr;
			XmlSerializer serializer;
			IXmlType xmlType;
			switch (type)
			{
			case XmlTypes.Config:
				// do deserializing of configurations
				serializer = new XmlSerializer(typeof(Configurations));
				sr = new StreamReader(configsXmlFile);
				xmlType = (Configurations)serializer.Deserialize(sr);
				break;
			default: // case XmlTypes.Fastcut:
				// do deserializing of fastcuts
				serializer = new XmlSerializer(typeof(Fastcuts));
				sr = new StreamReader(fastcutXmlFile);
				xmlType = (Fastcuts)serializer.Deserialize(sr);
				break;
			}

			sr.Close();
			return xmlType;
		}

		/// <summary>
		/// Saves the specified <paramref name="type"/> in a XML file.
		/// </summary>
		/// <param name="type">The type to save.</param>
		public void SaveXml(IXmlType type)
		{
			FileStream fs;
			XmlSerializer serializer;
			switch (type.XmlType)
			{
			case XmlTypes.Config:
				// do serializing of configurations
				serializer = new XmlSerializer(typeof(Configurations));
				fs = new FileStream(configsXmlFile, FileMode.Create);                    
				break;
			default: // case XmlTypes.Fastcut:
				// do serializing of fastcuts
				serializer = new XmlSerializer(typeof(Fastcuts));
				fs = new FileStream(fastcutXmlFile, FileMode.Create);
				break;
			}
			serializer.Serialize(fs, type);
			fs.Close();
		}
	}
}

