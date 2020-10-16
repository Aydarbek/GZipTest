using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class HeaderedFilePreparer
    {
        const int partSize = 1048576;
        static ReaderWriterLock locker = new ReaderWriterLock();

        FileInfo processingFile;
        long offset;
        int part;

        int headerSize { get { return FileHeaderHandler.HEADER_SIZE; } }

        public HeaderedFilePreparer(FileInfo processingFile, long offset, int part)
        {
            this.processingFile = processingFile;
            this.offset = offset;
            this.part = part;
        }

        internal void PrepareHeaderedFile()
        {
            try
            {
                Console.WriteLine($"{Thread.CurrentThread.Name} start. {DateTime.Now.ToString("HH:mm:ss.fff")}");

                byte[] bytes = new byte[Math.Min(partSize, processingFile.Length - offset)];
                bool isEndOfFile = bytes.Length < partSize ? true : false;

                FileHeader fileHeader = new FileHeader(part, bytes.Length, isEndOfFile);

                using (FileStream fileToProcessStream = processingFile.OpenRead())
                {
                    fileToProcessStream.Seek(offset, SeekOrigin.Begin);
                    fileToProcessStream.Read(bytes, 0, bytes.Length);

                    byte[] zipBytes = Compress(bytes);
                    OutputStreamQueuer.GetInstance().WriteBytesToQueue(part, zipBytes, isEndOfFile);
                }

                Console.WriteLine($"{Thread.CurrentThread.Name} finished. {DateTime.Now.ToString("HH:mm:ss.fff")}");
            }
            catch (Exception)
            {
                throw;
            }

        }

        internal byte[] Compress(byte[] inputBytes)
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
