using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Documents;

namespace XPress.Web
{
    /// <summary>
    /// Contains a javascript json extentions.
    /// </summary>
    public static class JavascriptJsonExtentions
    {
        /// <summary>
        /// Converts the object into javascript json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string ToJSJson<T>(this T o, bool isPreety =
#if DEBUG
 true)
#else 
            false)
#endif
        {
            return XPress.Serialization.Javascript.JsonStringSerializer.Global.Serialize(o, true, isPreety);
        }

        /// <summary>
        /// Converts the json source into an object using the string json serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T FromJSJson<T>(this string source)
        {
            return XPress.Serialization.Javascript.JsonStringSerializer.Global.Deserialize<T>(source, true);
        }

        /// <summary>
        /// Converts the json value into an object using the string json serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T FromJSJson<T>(this IJsonValue<string> val)
        {
            return XPress.Serialization.Javascript.JsonStringSerializer.Global.Deserialize<T>(val, true);
        }

        /// <summary>
        /// Converts the json source into an object using the string json serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object FromJSJson(this string source, Type t)
        {
            return XPress.Serialization.Javascript.JsonStringSerializer.Global.Deserialize(source, t, true);
        }

        /// <summary>
        /// Converts the json value into an object using the string json serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object FromJSJson(this IJsonValue<string> val, Type t)
        {
            return XPress.Serialization.Javascript.JsonStringSerializer.Global.Deserialize(val, t, true);
        }
    }
}
