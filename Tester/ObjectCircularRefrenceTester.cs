using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPress.Serialization.Attributes;

namespace Tester
{
    /// <summary>
    /// Implements a circular refrence test.
    /// </summary>
    [XPressMemberSelection(XPressMemberSelectionType.OptIn)]
    public class ObjectCircularRefrenceTester
    {
        public ObjectCircularRefrenceTester(int level, ObjectCircularRefrenceTester parent)
        {
            Parent = parent;
            LevelIndex = level;
        }

        [XPressMember("Parent")]
        public ObjectCircularRefrenceTester Parent { get; private set; }

        [XPressMember("Child")]
        public ObjectCircularRefrenceTester Child { get; set; }

        [XPressMember("level")]
        public int LevelIndex { get; private set; }
    }
}
