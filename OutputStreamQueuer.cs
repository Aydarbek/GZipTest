using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class OutputStreamQueuer
    {
        ConcurrentQueue<ZipBlock> streamQueue = new ConcurrentQueue<ZipBlock>();
        internal FileInfo outputFile { get; set; }
        bool isEndOfFile;
        private static OutputStreamQueuer outputStreamQueuer;

        private OutputStreamQueuer()  {}

        internal static OutputStreamQueuer GetInstance()
        {
            if (outputStreamQueuer == null)
                outputStreamQueuer = new OutputStreamQueuer();

            return outputStreamQueuer;
        }

        internal void WriteBytesToQueue(int blockNum, byte[] inputBytes, bool isEndOfFile)
        {
            streamQueue.Enqueue(new ZipBlock(blockNum, inputBytes, isEndOfFile));
            
            if (isEndOfFile)
                this.isEndOfFile = true;
        }

        internal void WriteStreamBytesToFile()
        {
            using (FileStream outputFileStream = outputFile.Create())
            {
                while (true)
                {
                    if (!streamQueue.IsEmpty)
                    {
                        ZipBlock nextBlock;
                        streamQueue.TryDequeue(out nextBlock);

                        FileHeader fileHeader = new FileHeader(nextBlock.blockNum, nextBlock.blockData.Length, nextBlock.isEndOfFile);
                        FileHeaderHandler.WriteFileHeader(outputFileStream, fileHeader);

                        outputFileStream.Write(nextBlock.blockData, 0, nextBlock.blockData.Length);
                        Thread.Sleep(100);
                    }

                    else
                    {
                        if (isEndOfFile)
                            break;

                        Console.WriteLine("Waiting for input bytes");
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
