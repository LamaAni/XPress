using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Coding
{
    /// <summary>
    /// Combines multiple hash codes into one.
    /// </summary>
    public class HashCodeCombiner
    {
        private int currentHash;

        public HashCodeCombiner() : this(0x1505) { }

        public HashCodeCombiner(int initialValue)
        {
            this.currentHash = initialValue;
        }

        public static int Combine(int code1, int code2)
        {
            return ((code1 << 5) + code1) ^ code2;
        }

        public HashCodeCombiner Add(int value)
        {
            this.currentHash = HashCodeCombiner.Combine(this.currentHash, value);
            return this;
        }

        public int CurrentHash
        {
            get { return this.currentHash; }
        }

        public static int Combine(int code1, int code2, int code3)
        {
            return HashCodeCombiner.Combine(HashCodeCombiner.Combine(code1, code2), code3);
        }

        public static int Combine(int code1, int code2, int code3, int code4)
        {
            return HashCodeCombiner.Combine(HashCodeCombiner.Combine(code1, code2), HashCodeCombiner.Combine(code3, code4));
        }

        public static HashCodeCombiner Start()
        {
            return new HashCodeCombiner();
        }
    }
}
