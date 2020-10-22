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
    class CompressionQueuer : IStreamQueuer
    {
        ConcurrentQueue<FileBlock> streamQueue = new ConcurrentQueue<FileBlock>();
        public Action ShowResult { get; set; }
        public FileInfo outputFile { get; set; }
        FileBlock nextBlock;
        FileHeader fileHeader;
        bool isEndOfFile;

        public void PutBytesToQueue(int blockNum, byte[] bytes, bool isEndOfFile)
        {
            streamQueue.Enqueue(new FileBlock(blockNum, bytes, isEndOfFile));
            
            if (isEndOfFile)
                this.isEndOfFile = true;
        }

        public void WriteBytesToFile()
        {
            using (FileStream outputFileStream = outputFile.Create())
            {
                while (true)
                {
                    if (!streamQueue.IsEmpty)
                    {
                        streamQueue.TryDequeue(out nextBlock);

                        fileHeader = new FileHeader(nextBlock.blockNum, nextBlock.blockData.Length, nextBlock.isEndOfFile);
                        FileHeaderHelper.WriteFileHeader(outputFileStream, fileHeader);

                        outputFileStream.Write(nextBlock.blockData, 0, nextBlock.blockData.Length);
                    }

                    else
                    {
                        if (isEndOfFile)
                        {
                            Thread.Sleep(1000);
                            if (streamQueue.IsEmpty)
                            {
                                ShowResult();
                                break;
                            }
                        }

                        Thread.Sleep(100);
                    }
                }
            }
        }
    }
}
