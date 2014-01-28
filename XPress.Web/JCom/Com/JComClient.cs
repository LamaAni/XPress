using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization;
using XPress.Web.JCom.Com.Request;
using XPress.Serialization.Attributes;

namespace XPress.Web.JCom.Com
{
    /// <summary>
    /// Implements a json communication client to allow passing of variables to the command values.
    /// </summary>
    public class JComClient
    {
        /// <summary>
        /// Creates the jcom client
        /// </summary>
        /// <param name="source">The object->id id->object mapper</param>
        /// <param name="compiler">The compiler from which to compile the code/script. Default is: Compilers.Specialized.JavaScriptCompiler.Global </param>
        public JComClient(IJComObjectSource source, Compilers.Compiler compiler = null)
            : base()
        {
            ObjectSource = source;
            Compiler = compiler == null ? Compilers.Specialized.JavaScriptCompiler.Global : compiler;
        }

        static JComClient()
        {
            // registering object generators.
            JavascriptClient.Request.JClientRequest.AddTranslator("JCOM", (doc, client) =>
            {
                JComRequestCommand rq = doc.FromJSJson<JComRequestCommand>();
                rq.JClient = client.JComClient;
                return rq;
            });
        }

        /// <summary>
        /// Validates that the static mehods have been initialzied.
        /// </summary>
        public static void ValidateInitialized()
        { 
        }

        public Compilers.Compiler  Compiler { get; private set; }

        /// <summary>
        /// The object->id id->object mapper
        /// </summary>
        public IJComObjectSource ObjectSource { get; private set; }
    }
}
