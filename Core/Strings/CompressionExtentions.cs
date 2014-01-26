using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPress.Strings
{
    public static class CompressionExtentions
    {
        public static string CompressJavascript(this string source)
        {
            return Yahoo.Yui.Compressor.JavaScriptCompressor.Compress(source);
        }

        public static string CompressCss(this string source)
        {
            return Yahoo.Yui.Compressor.CssCompressor.Compress(source, 200, Yahoo.Yui.Compressor.CssCompressionType.StockYuiCompressor);
        }
    }
}
