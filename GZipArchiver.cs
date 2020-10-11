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
        public static void Compress(FileInfo sourceFile)
        {
            using (FileStream sourceFileStream = sourceFile.OpenRead())
            {
                using (FileStream compressedFile = File.Create(sourceFile.FullName + ".gzip"))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFile, CompressionMode.Compress))
                    {
                        sourceFileStream.CopyTo(compressionStream);
                    }
                }

                FileInfo info = new FileInfo(sourceFile.Name + ".gzip");
                Console.WriteLine($"Compressed {sourceFile.Name} from {sourceFile.Length.ToString()} to {info.Length.ToString()} bytes.");
            }
        }

        public static void Decompress(FileInfo zipFile)
        {
            using (FileStream zipFileStream = zipFile.OpenRead())
            {
                string zipFileName = zipFile.FullName;
                string unzippedFileName = zipFileName.Remove(zipFileName.Length - zipFile.Extension.Length);

                using (FileStream unpackedFile = File.Create(unzippedFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(zipFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(unpackedFile);
                        Console.WriteLine($"File {zipFile.Name} decompressed");
                    }
                }
            }
        }
    }
}
