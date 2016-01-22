namespace Picturez_Lib
{
    using System;
    using System.Collections.Generic;

    /// <summary> Contains a list with all stored configurations. </summary>
    public class Configurations : IXmlType
    {        
        /// <summary> All stored configurations. </summary>
        public List<Configuration> Configs { get; set; }

        /// <summary> The configuration of Picturez' last usage. </summary>
        public Configuration CurrentConfig { get; set; }

        #region IXmlType Member
        [NonSerialized] private readonly XmlTypes xmlType;
        /// <summary>
        ///  The xml class type; value is <see cref="XmlTypes.Config"/>.
        /// </summary>
        public XmlTypes XmlType { get{ return xmlType;} }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Configurations"/> class.
        /// </summary>
        public Configurations()
        {
            xmlType = XmlTypes.Config;
            CurrentConfig = new Configuration();
			CurrentConfig.SetDefaultValues ();
            Configs = new List<Configuration>();
        }

        /// <summary>
        /// Adds the specified <paramref name="configuration"/> to 
        /// <see cref="Configs"/> as first list element.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void AddAsFirst(Configuration configuration)
        {
            // Remove(configuration);
            Configs.Insert(0, configuration);
        }

        /// <summary>
        /// Checks, if <paramref name="foreign"/> already exists in 
        /// <see cref="Configs"/>.
        /// </summary>
        /// <param name="foreign">The configuration to check.</param>
        /// <returns>True, if <paramref name="foreign"/> already exists in 
        /// <see cref="Configs"/>.</returns>
        public bool Exists(string foreign)
        {
            foreach (var config in Configs)
            {
                if (config.Name == foreign)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the <see name="Configuration"/> with the specified 
        /// <paramref name="name"/> in <see cref="Configs"/>.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>
        /// The <see name="Configuration"/> to find. 
        /// If <see name="Configuration"/> is not found, it returns null, no 
        /// exception is thrown.
        /// </returns>
        public Configuration Find(string name)
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                var config = Configs[i];
                if (config.Name == name)
                {
                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the <see name="Configuration"/> with the specified 
        /// <paramref name="name"/> in <see cref="Configs"/> and returns 
        /// its index position.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>
        /// The index position of the <see name="Configuration"/> in 
        /// <see cref="Configs"/>. 
        /// If <see name="Configuration"/> is not found, it returns -1.
        /// </returns>
        public int FindIndex(string name)
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                var config = Configs[i];
                if (config.Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes <paramref name="configuration"/> in 
        /// <see cref="Configs"/>. If <paramref name="configuration"/> is not 
        /// found in <see cref="Configs"/>, no exception is thrown.
        /// </summary>
        /// <param name="configuration">The configuration to remove.</param>
        /// <returns>
        /// True, if the configuration was removed, otherwise false.</returns>
        public bool Remove(Configuration configuration)
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                if (Configs[i].Name == configuration.Name)
                {
                    Configs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the <see name="Configuration"/> with the specified 
        /// <paramref name="name"/>in <see cref="Configs"/>. 
        /// If the configuration is not found, no exception is thrown.
        /// </summary>
        /// <param name="name">The name of the configuration to remove.</param>
        /// <returns>
        /// True, if the configuration was removed, otherwise false.</returns>
        public bool Remove(string name)
        {
            for (int i = 0; i < Configs.Count; i++)
            {
                if (Configs[i].Name == name)
                {
                    Configs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
