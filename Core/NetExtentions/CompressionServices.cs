using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Compression
{
    public static class CompressionServices
    {
        public static byte[] Zip(this byte[] values, CompressionLevel level = CompressionLevel.Fastest)
        {
            MemoryStream strm = new MemoryStream(values);
            MemoryStream compressed = new MemoryStream();
            using (GZipStream zip = new GZipStream(compressed, level))
            {
                strm.CopyTo(zip);
            }

            byte[] cvals = compressed.ToArray();
            strm.Close();
            compressed.Close();
            return cvals;
        }

        public static byte[] UnZip(this byte[] values)
        {
            MemoryStream strm = new MemoryStream(values);
            MemoryStream decompressed = new MemoryStream();
            using (GZipStream zip = new GZipStream(strm, CompressionMode.Decompress))
            {
                zip.CopyTo(decompressed);
            }
            byte[] dvals = decompressed.ToArray();
            strm.Close();
            decompressed.Close();
            return dvals;
        }
    }
}
