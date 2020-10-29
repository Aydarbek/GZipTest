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
        static object locker = new object();    
        FileHeader currentHeader;
        byte[] zipBytes;


        public FileBlock ReadNextBlock(FileStream sourceZipFileStream)
        {
            try
            {
                lock (locker)
                {
                    if (sourceZipFileStream.Position == sourceZipFileStream.Length)
                        return new NullFileBlock();

                    currentHeader = FileHeaderHelper.ReadFileHeader(sourceZipFileStream);
                    zipBytes = new byte[currentHeader.blockSize];
                    sourceZipFileStream.Read(zipBytes, 0, zipBytes.Length);

                    ShowCurrentStatus(sourceZipFileStream);

                    return new FileBlock(currentHeader.blockNum, zipBytes, currentHeader.isEndOfFile);
                }
            }
            catch (Exception ex)
            {
                Archivator.threadException = ex;
                return new NullFileBlock();
            }
        }

        private void ShowCurrentStatus(FileStream sourceZipFileStream)
        {
            float progress = (float)sourceZipFileStream.Position / (float)sourceZipFileStream.Length * 100;
            Console.Write($"\rProcessed {sourceZipFileStream.Position / 1024}/{sourceZipFileStream.Length / 1024} Kbytes ({progress:0.0}%)");
        }

    }
}
