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
        ConcurrentQueue<byte[]> streamQueue = new ConcurrentQueue<byte[]>();
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

        internal void WriteBytesToQueue(byte[] inputBytes, bool isEndOfFile)
        {
            streamQueue.Enqueue(inputBytes);
            this.isEndOfFile = isEndOfFile;
        }

        internal void WriteStreamBytesToFile()
        {
            using (FileStream outputFileStream = outputFile.Create())
            {
                while (true)
                {
                    if (!streamQueue.IsEmpty)
                    {
                        byte[] nextBlock;
                        streamQueue.TryDequeue(out nextBlock);
                        outputFileStream.Write(nextBlock, 0, nextBlock.Length);
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
