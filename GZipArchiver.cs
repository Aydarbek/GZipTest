using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class GZipArchiver
    {
        internal static byte[] Compress(byte[] inputBytes)
        {
            using(MemoryStream outStream = new MemoryStream())
            {
                using (MemoryStream inStream = new MemoryStream(inputBytes))
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress))
                {
                    inStream.CopyTo(zipStream);
                }
                return outStream.ToArray();
            }
        }

        internal static byte[] Decompress(byte[] inputBytes)
        {
            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(outStream);
                    return outStream.ToArray();
                }
            }
        }
    }
}
