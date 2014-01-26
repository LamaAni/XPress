
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;

namespace Tester
{

    public class NestedTestObject
    {
        public NestedTestObject(NestedTestObject intern, int nestIndex)
        {
            Intern = intern;
            SomeText = "lama";
            m_getNestIndex = () => nestIndex.ToString();
        }

        [XPressMember]
        public NestedTestObject Intern { get; set; }

        [XPressMember]
        public string RandId { get; set; }

        [XPressMember]
        public string SomeText { get; set; }

        [XPressMember]
        public Func<string> m_getNestIndex { get; set; }

        public override string ToString()
        {
            return m_getNestIndex();
        }


        public static NestedTestObject CreateNested(int nestDepth)
        {
            NestedTestObject cur = null;

            for (var i = 0; i < nestDepth; i++)
            {
                cur = new NestedTestObject(cur, i);
            }

            return cur;
        }

    }
}
