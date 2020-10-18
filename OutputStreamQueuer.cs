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

                        //Console.Write($"\rWriting block {nextBlock.blockNum}.         {DateTime.Now.ToString("HH:mm:ss.fff")}");
                        outputFileStream.Write(nextBlock.blockData, 0, nextBlock.blockData.Length);
                        outputFileStream.Flush();
                    }

                    else
                    {
                        if (isEndOfFile)
                        {
                            //Console.WriteLine("\rFinish compressing.");
                            break;
                        }

                        //Console.WriteLine("\rWaiting for input bytes");
                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
