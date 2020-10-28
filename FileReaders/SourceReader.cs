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
        static object locker = new object();
        int blockNum; 
        bool isEndOfFile;
        byte[] block;


        public FileBlock ReadNextBlock(FileStream sourceFileStream)
        {
            try
            {
                lock (locker)
                {
                    if (sourceFileStream.Position == sourceFileStream.Length)
                        return new NullFileBlock();

                    block = new byte[Math.Min(partSize, sourceFileStream.Length - sourceFileStream.Position)];
                    sourceFileStream.Read(block, 0, block.Length);
                    ShowCurrentStatus(sourceFileStream);
                    isEndOfFile = sourceFileStream.Position == sourceFileStream.Length;
                    return new FileBlock(++blockNum, block, isEndOfFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new NullFileBlock();
            }
        }

        private void ShowCurrentStatus(FileStream sourceFileStream)
        {
            float progress = (float)sourceFileStream.Position / (float)sourceFileStream.Length * 100;
            Console.Write($"\rProcessed {sourceFileStream.Position / 1024}/{sourceFileStream.Length / 1024} Kbytes ({progress:0.0}%)");
        }
    }
}
