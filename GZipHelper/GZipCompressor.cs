using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace GZipTest
{
    class GZipCompressor : IGZipProcessor
    {
        public byte[] ProcessBytes(byte[] inputBytes)
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
    }
}
