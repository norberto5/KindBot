using System.Xml;

namespace KindBot
{
    public interface IConfigurable
    {
        string GetConfigurationFilename();
        bool LoadConfiguration(XmlReader xmlReader);
        void CreateDefaultConfiguration(XmlWriter xmlWriter);
    }
}