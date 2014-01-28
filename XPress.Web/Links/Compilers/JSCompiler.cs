using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using XPress.Web.Links.Attributes;

namespace XPress.Web.Links.Compilers
{
    /// <summary>
    /// Creates 
    /// </summary>
    public class JSCompiler : Compiler
    {
        /// <summary>
        /// Static collection of construction codes.
        /// </summary>
        static ConcurrentDictionary<Type, string> __constructionCodes = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// Creates the init Javascript code to run on the client side for the specified type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string CreateInitCode(LinkMapInfo lmi)
        {
            string code;
            if (!__constructionCodes.TryGetValue(lmi.MappedType, out code))
            {
                lock (__constructionCodes)
                {
                    if (!__constructionCodes.TryGetValue(lmi.MappedType, out code))
                    {
                        StringBuilder cmnd = new StringBuilder();

                        lmi.LinksThatNeedActivation.ForEach(l =>
                        {
                            if (l.Type == LinkType.Constructor || l.Type == LinkType.InitScriptFunction)
                            {
                                cmnd.Append("$EO$(this,'" + l.UniqueId + "');");//$.extend(this, $.XPress.Links.Loaded[]);if(this.$!=null)this.$();this.$=null;");
#if DEBUG
                                cmnd.Append("\n");
#endif
                            }
                        });

                        code = cmnd.ToString();
                        __constructionCodes.TryAdd(lmi.MappedType, code);
                    }
                }
            }
            return code;
        }

        static JSCompiler m_Global = new JSCompiler();

        /// <summary>
        /// The global js link compiler.
        /// </summary>
        public static JSCompiler Global
        {
            get { return JSCompiler.m_Global; }
        }

    }
}
