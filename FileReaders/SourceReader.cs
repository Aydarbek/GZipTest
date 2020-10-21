using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class SourceReader : IFileReader
    {
        const long partSize = 1048576;
        public FileInfo sourceFile { get; set; }
        static ReaderWriterLock readWriteLock = new ReaderWriterLock();
        static object locker = new object();
        long offset;
        int blockNum;
        
        public FileBlock ReadNextBlock()
        {
            try
            {
                byte[] block;
                lock (locker)
                {
                    using (FileStream sourceFileStream = sourceFile.OpenRead())
                    {
                        readWriteLock.AcquireReaderLock(int.MaxValue);

                        block = new byte[Math.Min(partSize, sourceFileStream.Length - offset)];

                        sourceFileStream.Seek(offset, SeekOrigin.Begin);

                        if (sourceFileStream.Position == sourceFileStream.Length)
                            return new NullFileBlock();

                        offset += block.Length;
                        sourceFileStream.Read(block, 0, block.Length);

                        bool isEndOfFile = offset == sourceFileStream.Length;

                        readWriteLock.ReleaseReaderLock();
                        ShowCurrentStatus();

                        return new FileBlock(++blockNum, block, isEndOfFile);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new NullFileBlock();
            }
        }

        private void ShowCurrentStatus()
        {
            float progress = (float)offset / (float)sourceFile.Length * 100;
            Console.Write($"\rProcessed {offset / 1024}/{sourceFile.Length / 1024} Kbytes ({progress:0.0}%)");
        }
    }
}
