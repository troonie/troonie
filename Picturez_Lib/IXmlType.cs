namespace Picturez_Lib
{
    /// <summary>
    /// Interface for classes, which are able to store in Picturez' xml file 
    /// system.
    /// </summary>
    public interface IXmlType
    {
        /// <summary> The class type for storing in xml file. </summary>
        XmlTypes XmlType { get; }
    }
}
