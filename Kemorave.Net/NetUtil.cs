using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Kemorave.Net
{
    public static class NetUtil
    {
        public static string JsonSerialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public static T jsonDeserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
        public static void XmlSerialize(object objectToSave, string filePath)
        {
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                XmlSerializer xs = new XmlSerializer(objectToSave.GetType());
                xs.Serialize(wr, objectToSave);
            }
        }
        public static object XmlDeserialize(Type type, string filePath)
        {
            using (System.IO.TextReader reader = new System.IO.StreamReader(filePath))
            {
                XmlSerializer deserializer = new XmlSerializer(type);
                return deserializer.Deserialize(reader);
            }
        }

    }
}