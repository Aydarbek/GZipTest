﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class FileRestorer
    {
        ConcurrentDictionary<int, byte[]> fileStreams = new ConcurrentDictionary<int, byte[]>();
        FileInfo resultFile;
        bool isEndOfFile { get; set; }

        int currentWaitingBlock = 1;

        public FileRestorer(FileInfo resultFile)
        {
            this.resultFile = resultFile;
        }

        internal void PutFileBytes(int blockNum, byte[] fileStream, bool isEndOfFile)
        {
            fileStreams.TryAdd(blockNum, fileStream);
            this.isEndOfFile = isEndOfFile;
        }

        internal void WriteFileBytesToStream()
        {
            using (FileStream resultFileStream = resultFile.OpenWrite())
            {
                while (true)
                {
                    byte[] currentWaitingStream;
                    if (fileStreams.TryGetValue(currentWaitingBlock, out currentWaitingStream))
                    {
                        Console.WriteLine($"Try to copy block {currentWaitingBlock}");
                        resultFileStream.Write(currentWaitingStream, 0, currentWaitingStream.Length);
                        fileStreams.TryRemove(currentWaitingBlock, out currentWaitingStream);
                        currentWaitingBlock++;

                        if (isEndOfFile && fileStreams.IsEmpty)
                            break;
                    }

                    else
                    {
                        Console.WriteLine($"Waiting for next block: number {currentWaitingBlock}");
                        Thread.Sleep(100);
                    }
                } 
            }
        }
    }
}