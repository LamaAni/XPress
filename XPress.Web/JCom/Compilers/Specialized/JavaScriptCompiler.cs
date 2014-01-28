using XPress.Web.JCom.Com.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Web.JCom.Compilers.Specialized
{
    public class JavaScriptCompiler : Compiler
    {
        public JavaScriptCompiler()
        {
        }

        static JavaScriptCompiler()
        {
            Global = new JavaScriptCompiler();
        }

        public static JavaScriptCompiler Global { get; private set; }

        static ConcurrentDictionary<Type, string> m_typeDefs = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Generates the type definition codes.
        /// </summary>
        /// <param name="ti">Extending type definitions.</param>
        /// <returns></returns>
        public override string CreateTypeDef(Map.JComTypeInfo ti)
        {
            string rsp;
            if (!m_typeDefs.TryGetValue(ti.MappedType, out rsp))
            {
                lock (m_typeDefs)
                {
                    if (!m_typeDefs.TryGetValue(ti.MappedType, out rsp))
                    {
                        rsp = CreateTypeDefinitionCode(ti).ToString();
                        m_typeDefs.TryAdd(ti.MappedType, rsp);
                    }
                }
            }

            return rsp;
        }

        private static StringBuilder CreateTypeDefinitionCode(Map.JComTypeInfo ti)
        {
            // reading the type info and generating the code.
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
#if DEBUG
            sb.Append("\n");
#endif
            ti.DataMembers.ForEach(mi =>
            {
                sb.Append(mi.Name);

                bool doRead = true;
                bool doWrite = true;
                if (mi.IsProperty)
                {
                    PropertyInfo pi = (mi.MappedMember as PropertyInfo);
                    doRead = pi.CanRead;
                    doWrite = pi.CanWrite;
                }

                /*sb.Append("function(){this.$C$().MD(this,'" + mi.Name + "'," + mi.MemberAttribute.Synced.ToString().ToLower()
                    + ", arguments," + doRead.ToString().ToLower() + "," + doWrite.ToString().ToLower() + ");},");*/
                sb.Append(":function(){return $.XPress.JCOM.Property(this,'");
                sb.Append(mi.Name);
                sb.Append("',arguments,");
                sb.Append(mi.CanBeAsynchromized ? "true" : "false");
                sb.Append(',');
                sb.Append(doRead ? "true" : "false");
                sb.Append(',');
                sb.Append(doWrite ? "true" : "false");
                sb.Append(");},");
#if DEBUG
                sb.Append("\n");
#endif
            });
            ti.Methods.ForEach(mi =>
            {
                sb.Append(mi.Name);
                sb.Append(":function(){return $.XPress.JCOM.Method(this,'");
                sb.Append(mi.Name);
                sb.Append("',arguments,");
                sb.Append(mi.CanBeAsynchromized ? "true" : "false");
                sb.Append(");},");
#if DEBUG
                sb.Append("\n");
#endif
            });
#if DEBUG
            sb.Append("\n");
#endif
            sb.Append("}");
            return sb;
        }
    }
}
