using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Serialization.Reference
{
    public class RefrenceId
    {
        public RefrenceId(uint id)
        {
            Value = id;
        }

        public uint Value { get; private set; }
    }
}
