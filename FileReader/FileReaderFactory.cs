using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class FileReaderFactory
    {
        internal static IFileReader GetFileReader(string operation)
        {
            switch (operation)
            {
                case "compress":
                    return new FileReader();
                case "decompress":
                    return new ZipReader();
                default:
                    return null;
            }
        }
    }
}
