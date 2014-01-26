using XPress.Serialization.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Javascript
{
    /// <summary>
    /// A collection of global values that apply to the string json serialzier.
    /// </summary>
    public static class JsonStringSerialzierGlobals
    {
        /// <summary>
        /// Static construct.
        /// </summary>
        static JsonStringSerialzierGlobals()
        {
            SerializationDefinitions<string> def = new SerializationDefinitions<string>(Core.JavscriptJson.JsonStringGlobals.JsonDefintions, "tp");

            m_Definitions = def;
        }

        static SerializationDefinitions<string> m_Definitions;

        public static SerializationDefinitions<string> Definitions
        {
            get
            {
                return m_Definitions;
            }
        }
    }
}
