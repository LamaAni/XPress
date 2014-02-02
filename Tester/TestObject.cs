using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;
using XPress.Serialization.Documents;

namespace Tester
{
    /// <summary>
    /// Implements a test object for serialization purpuse.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class TestObject
    {
        public TestObject()
        {
            thistype = this.GetType();
            string externalTarget = "extern";
            internalFunc = (s) => externalTarget + " :" + s;
            _pdList = new PostDeserialize<List<string>>(new List<string>(new string[] { "a", "b" }));
        }

        [XPressMember("linqfunc")]
        Func<string,string> internalFunc;

        [XPressMember("lst")]
        List<string> lst = new List<string>(new string[] { "a", "b", "c", "d" });

        [XPressMember("str")]
        string str = "alna";

        [XPressMember("date")]
        DateTime dt = DateTime.Now;

        [XPressMember("type")]
        Type thistype;

        [XPressMember("pdlist")]
        public PostDeserialize<List<string>> _pdList;

        [XPressMember("enum")]
        public TestEnum ValueEnum = TestEnum.Value2;

        public List<string> PdList
        {
            get { return _pdList.Value; }
        }

        public override string ToString()
        {
            return internalFunc(base.ToString());
        }
    }

    public enum TestEnum { Value1, Value2 }
}
