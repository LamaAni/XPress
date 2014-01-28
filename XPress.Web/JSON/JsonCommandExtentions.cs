using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XPress.Serialization;
namespace XPress.Web
{
    public static class JsonCommandExtentions
    {
        public static string ToJson(this JSON.IJsonCommand cmd)
        {
            return cmd.ToJSJson();
        }

        public static T ReadCommandFromStream<T>(this HttpRequest request)
            where T : class, JSON.IJsonCommand
        {
            StreamReader reader = new StreamReader(request.InputStream);
            string json = reader.ReadToEnd();
            reader.Close();
            T val = json.FromJSJson<T>(); //Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return val;
        }
    }
}
