using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.GZipHelper
{
    class GZipProcessorFactory
    {
        internal static IGZipProcessor GetGZipProcessor(string operation)
        {
            switch (operation)
            {
                case "compress":
                    return new GZipCompressor();
                case "decompress":
                    return new GZipDecompressor();
                default:
                    return null;
            }
        }
    }
}
