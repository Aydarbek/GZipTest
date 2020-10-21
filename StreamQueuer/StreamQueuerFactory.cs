using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class StreamQueuerFactory
    {
        internal static IStreamQueuer GetStreamQueuer(string operation)
        {
            switch (operation)
            {
                case "compress":
                    return new CompressionQueuer();
                case "decompress":
                    return new DecompressionQueuer();
                default:
                    return null;
            }
        }
    }
}
