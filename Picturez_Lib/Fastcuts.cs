namespace Picturez_Lib
{
    using System;
    using System.Collections.Generic;

    /// <summary> Contains a list with all stored configurations. </summary>
    public class Fastcuts : IXmlType
    {
        /// <summary> All stored fastcuts. </summary>
        public List<Fastcut> All { get; set; }

        /// <summary> The facstcut of Picturez' last usage. </summary>
        public Fastcut Current { get; set; }

        #region IXmlType Member
        [NonSerialized]
        private readonly XmlTypes xmlType;
        /// <summary>
        ///  The xml class type; value is <see cref="XmlTypes.Fastcut"/>.
        /// </summary>
        public XmlTypes XmlType { get { return xmlType; } }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Fastcuts"/> class.
        /// </summary>
        public Fastcuts()
        {
            xmlType = XmlTypes.Fastcut;
            Current = new Fastcut();
            All = new List<Fastcut>();            
        }

        /// <summary>
        /// Adds the specified <paramref name="fastcut"/> to 
        /// <see cref="All"/> as first list element.
        /// </summary>
        /// <param name="fastcut">The fastcut to add.</param>
        public void AddAsFirst(Fastcut fastcut)
        {
            // Remove(configuration);
            All.Insert(0, fastcut);
        }

        /// <summary>
        /// Checks, if <paramref name="foreign"/> already exists in 
        /// <see cref="All"/>.
        /// </summary>
        /// <param name="foreign">The configuration to check.</param>
        /// <returns>True, if <paramref name="foreign"/> already exists in 
        /// <see cref="All"/>.</returns>
        public bool Exists(string foreign)
        {
            foreach (var config in All)
            {
                if (config.Name == foreign)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Finds the <see name="Configuration"/> with the specified 
        /// <paramref name="name"/> in <see cref="All"/>.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>
        /// The <see name="Configuration"/> to find. 
        /// If <see name="Configuration"/> is not found, it returns null, no 
        /// exception is thrown.
        /// </returns>
        public Fastcut Find(string name)
        {
            for (int i = 0; i < All.Count; i++)
            {
                var fastcut = All[i];
                if (fastcut.Name == name)
                {
                    return fastcut;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the <see name="Configuration"/> with the specified 
        /// <paramref name="name"/> in <see cref="All"/> and returns 
        /// its index position.
        /// </summary>
        /// <param name="name">The name of the configuration to find.</param>
        /// <returns>
        /// The index position of the <see name="Configuration"/> in 
        /// <see cref="All"/>. 
        /// If <see name="Configuration"/> is not found, it returns -1.
        /// </returns>
        public int FindIndex(string name)
        {
            for (int i = 0; i < All.Count; i++)
            {
                var config = All[i];
                if (config.Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes <paramref name="fastcut"/> in 
        /// <see cref="All"/>. If <paramref name="fastcut"/> is not 
        /// found in <see cref="All"/>, no exception is thrown.
        /// </summary>
        /// <param name="fastcut">The fastcut to remove.</param>
        /// <returns>
        /// True, if the fastcut was removed, otherwise false.</returns>
        public bool Remove(Fastcut fastcut)
        {
            return Remove(fastcut.Name);
        }

        /// <summary>
        /// Removes the <see name="Fastcut"/> with the specified 
        /// <paramref name="name"/>in <see cref="All"/>. 
        /// If the fastcut is not found, no exception is thrown.
        /// </summary>
        /// <param name="name">The name of the fastcut to remove.</param>
        /// <returns>
        /// True, if the fastcut was removed, otherwise false.</returns>
        public bool Remove(string name)
        {
            for (int i = 0; i < All.Count; i++)
            {
                if (All[i].Name == name)
                {
                    All.RemoveAt(i);
                    if (Current.Name == name)
                    {
                        Current.Name = "";
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
