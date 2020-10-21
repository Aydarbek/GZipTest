using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class ZipReader : IFileReader
    {
        static ReaderWriterLock rwl = new ReaderWriterLock();
        static object locker = new object();
        public FileInfo sourceFile { get; set; }
        private long offset;
        FileHeader currentHeader;


        public FileBlock ReadNextBlock()
        {
            lock (locker)
            {
                using (FileStream sourceZipFileStream = sourceFile.OpenRead())
                {
                    rwl.AcquireReaderLock(int.MaxValue);
                    sourceZipFileStream.Seek(offset, SeekOrigin.Begin);

                    if (sourceZipFileStream.Position == sourceZipFileStream.Length)
                        return new NullFileBlock();

                    currentHeader = FileHeaderHandler.ReadFileHeader(sourceZipFileStream);
                    byte[] zipBytes = new byte[currentHeader.blockSize];
                    sourceZipFileStream.Read(zipBytes, 0, zipBytes.Length);

                    offset += FileHeaderHandler.HEADER_SIZE + currentHeader.blockSize;

                    ShowCurrentStatus();

                    rwl.ReleaseReaderLock();

                    return new FileBlock(currentHeader.blockNum, zipBytes, currentHeader.isEndOfFile);
                }
            }
        }

        private void ShowCurrentStatus()
        {
            float progress = (float)offset / (float)sourceFile.Length * 100;
            Console.Write($"\rDecompressed {offset / 1024}/{sourceFile.Length / 1024} Kbytes ({progress:0.0}%)");
        }
    }
}
