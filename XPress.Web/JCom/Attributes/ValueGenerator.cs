using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMC.Web.Attributes
{
    public abstract class ValueGenerator
    {
        public abstract object Generate(object o);
    }
}
