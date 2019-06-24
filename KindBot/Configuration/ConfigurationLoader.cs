using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using KindBot.Tools;

namespace KindBot.Configuration
{
    public static class ConfigurationLoader
    {
#if LINUX
        private const string configPath = @"config/";
#else
        private const string configPath = @"config\";
#endif
        private static readonly List<IConfigurable> _list = new List<IConfigurable>();

        public static bool LoadConfiguration()
        {
            bool success = true;
            try
            {
                foreach(IConfigurable configurable in _list)
                {
                    if(configurable.GetConfigurationFilename() == null) throw new NullReferenceException("GetConfigurationFilename() returned a null");

                    //create directory if it doesn't exist
                    string pathdir = Path.GetDirectoryName(configPath);
                    if(pathdir.Length > 0) Directory.CreateDirectory(pathdir);

                    string path = configPath + configurable.GetConfigurationFilename() + ".xml";
                    if(!File.Exists(path))
                    {
                        var xmlWriterSettings = new XmlWriterSettings()
                        {
                            Indent = true,
                            IndentChars = "\t",
                            NewLineOnAttributes = false,
                            OmitXmlDeclaration = true,
                            Encoding = System.Text.Encoding.UTF8
                        };

                        using(var w = XmlWriter.Create(path, xmlWriterSettings))
                        {
                            w.WriteStartDocument();
                            configurable.CreateDefaultConfiguration(w);
                            w.WriteEndDocument();
                            w.Close();
                        }
                        ConsoleEx.Warning($"Default configuration file {path} was created, because it was missing.");
                    }
                    else
                    {
                        using(var r = XmlReader.Create(path))
                        {
                            if(!configurable.LoadConfiguration(r)) success = false;
                        }
                    }
                }
                return success;
            }
            catch(Exception ex)
            {
                ConsoleEx.Error($"[Configuration]: An exception occured! Error message: {ex.Message}");
                return false;
            }
        }

        public static void AddConfigurationFile(IConfigurable configurable) => _list.Add(configurable);

        /// <summary>
        /// Writes list of elements to XML file.
        /// </summary>
        public static void WriteElementList(XmlWriter xml, string[,] elements)
        {
            for(int i = 0; i < elements.GetLength(0); i++)
            {
                WriteNewElement(xml, elements[i, 0], elements[i, 1], elements[i, 2]);
            }
        }

        /// <summary>
        /// Writes an element to XML file.
        /// </summary>
        public static void WriteNewElement(XmlWriter xml, string name, string variable, string comment = null)
        {
            if(comment != null) xml.WriteComment(" " + comment + " ");
            xml.WriteStartElement(name);
            xml.WriteString(variable);
            xml.WriteEndElement();
        }
    }
}