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
    class CompressionQueuer
    {
        ConcurrentQueue<FileBlock> streamQueue = new ConcurrentQueue<FileBlock>();
        internal FileInfo outputFile { get; set; }
        bool isEndOfFile;
        private static CompressionQueuer compressionQueuer;

        private CompressionQueuer()  {}

        internal static CompressionQueuer GetInstance()
        {
            if (compressionQueuer == null)
                compressionQueuer = new CompressionQueuer();

            return compressionQueuer;
        }

        internal void WriteBytesToQueue(int blockNum, byte[] inputBytes, bool isEndOfFile)
        {
            streamQueue.Enqueue(new FileBlock(blockNum, inputBytes, isEndOfFile));
            
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
                        FileBlock nextBlock;
                        streamQueue.TryDequeue(out nextBlock);

                        FileHeader fileHeader = new FileHeader(nextBlock.blockNum, nextBlock.blockData.Length, nextBlock.isEndOfFile);
                        FileHeaderHandler.WriteFileHeader(outputFileStream, fileHeader);
                                                
                        outputFileStream.Write(nextBlock.blockData, 0, nextBlock.blockData.Length);
                    }

                    else
                    {
                        if (isEndOfFile)
                        {
                            Archivator.GetInstance().ShowTimeResult();
                            break;
                        }

                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
