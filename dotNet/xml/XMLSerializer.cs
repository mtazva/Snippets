using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Utility
{

    /// <summary>
    /// XML object serialization
    /// </summary>
    public class Serializer
    {

        #region Serialize

        /// <summary>
        /// Convert object to unformatted serialized xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T target)
        {
            return SerializeObject<T>(target, false);
        }

        /// <summary>
        /// Convert object to serialized xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pObject"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(T target, bool formatResult)
        {
            return SerializeObject(target, typeof(T), formatResult);
        }

        /// <summary>
        /// Convert object to serialized xml string
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <param name="formatResult"></param>
        /// <returns></returns>
        public static string SerializeObject(object target, Type type, bool formatResult)
        {
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(type);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

            //if formatting is requested, set text writer format options
            if (formatResult)
            {
                xmlTextWriter.Formatting = Formatting.Indented;
                xmlTextWriter.IndentChar = '\t';
                xmlTextWriter.Indentation = 1;
            }

            xs.Serialize(xmlTextWriter, target);
            xmlTextWriter.Flush();
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;

            return UTF8Encoding.UTF8.GetString(memoryStream.ToArray()).Trim();
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Create object from serialized xml string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pXmlizedString"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(String serializedValue)
        {
            return (T)DeserializeObject(serializedValue, typeof(T));
        }

        /// <summary>
        /// Create object from serialized xml string
        /// </summary>
        /// <param name="serializedValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeObject(string serializedValue, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            MemoryStream memoryStream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(serializedValue));

            return xs.Deserialize(memoryStream);
        }

        #endregion

    }

}
