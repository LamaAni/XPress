using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XPress.Web.Links.Parsers.Css
{
    public class CssPrefixReplaceOptions
    {
        /// <summary>
        /// Create according to the context. (and browser type). Curently supports opera, ie, wekbit (any), blink, Gecko.
        /// </summary>
        /// <param name="context"></param>
        public CssPrefixReplaceOptions()
        {
            VendorPrefexs.Add("-o-");
            VendorPrefexs.Add("-ms-");
            VendorPrefexs.Add("-moz-");
            VendorPrefexs.Add("-webkit-");
        }

        public CssPrefixReplaceOptions(IEnumerable<string> vendorPrefexs)
        {
            VendorPrefexs = new HashSet<string>(vendorPrefexs);
        }

        public HashSet<string> VendorPrefexs { get; private set; }
    }
}
