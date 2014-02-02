using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;
using XPress.Serialization.Documents;

namespace Tester
{
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class PostDeserialzieTester
    {
        public PostDeserialzieTester()
        {
            __pdList = new PostDeserialize<List<string>>(new List<string>(new string[] { "a", "b" }));
        }

        [XPressMember]
        public PostDeserialize<List<string>> __pdList;

        public List<string> List { get { return __pdList.Value; } }
    }
}
