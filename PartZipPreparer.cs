using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest
{
    class PartZipPreparer
    {
        const int partSize = 20971520;  // 20 MegaBytes in Bytes
        FileInfo fileToSplit;
        int part;
        int offset;

        internal static Mutex mutexObj = new Mutex();

        public PartZipPreparer(FileInfo fileToSplit, int part, int offset)
        {
            this.fileToSplit = fileToSplit;
            this.part = part;
            this.offset = offset;
        }

        internal void PreparePartZip()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} start.");
            using (FileStream fileStream = fileToSplit.OpenRead())
            {
                using (FileStream zipFile = File.Create($"{fileToSplit.Name}_{part}.gzip."))
                {
                    using (GZipStream zipStream = new GZipStream(zipFile, CompressionMode.Compress))
                    {
                        byte[] bytes = new byte[Math.Min(partSize, fileToSplit.Length - offset)];

                        fileStream.Seek(offset, SeekOrigin.Begin);
                        fileStream.Read(bytes, 0, bytes.Length);
                        zipStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

    }
}
