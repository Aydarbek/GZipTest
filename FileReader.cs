using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class FileReader
    {
        const long partSize = 1048576;

        static ReaderWriterLock rwl = new ReaderWriterLock();
        static object locker = new object();
        internal FileInfo sourceFile { get; set; }
        private long offset = 0;
        private static FileReader fileReader;
        int blockNum = 0;

        private FileReader() {}

        internal static FileReader GetInstance()
        {
            if (fileReader == null)
                fileReader = new FileReader();

            return fileReader;
        }

        internal FileBlock ReadNextBlock()
        {
            lock (locker)
            {
                using (FileStream sourceFileStream = sourceFile.OpenRead())
                {
                    rwl.AcquireReaderLock(int.MaxValue);

                    if (offset != sourceFileStream.Length)
                        blockNum++;

                    byte[] block = new byte[Math.Min(partSize, sourceFileStream.Length - offset)];

                    float progress = (float)offset / (float)sourceFileStream.Length * 100;

                    Console.Write($"\rReaded {offset / 1024}/{sourceFileStream.Length / 1024} Kbytes ({progress:0.0}%)");


                    sourceFileStream.Seek(offset, SeekOrigin.Begin);
                    sourceFileStream.Read(block, 0, block.Length);

                    offset += block.Length;

                    bool isEndOfFile = offset == sourceFileStream.Length ? true : false;

                    rwl.ReleaseReaderLock();

                    return new FileBlock(blockNum, block, isEndOfFile);
                } 
            }
        }
    }
}
