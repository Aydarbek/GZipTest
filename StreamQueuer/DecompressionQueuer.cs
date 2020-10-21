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
    class DecompressionQueuer : IStreamQueuer
    {
        ConcurrentDictionary<int, byte[]> fileStreams = new ConcurrentDictionary<int, byte[]>();
        public FileInfo outputFile { get; set; }
        bool isEndOfFile { get; set; }
        int currentWaitingBlock = 1;
        byte[] currentWaitingStream;
        int waitCounter = 0;

        public void PutBytesToQueue(int blockNum, byte[] bytes, bool isEndOfFile)
        {
            fileStreams.TryAdd(blockNum, bytes);
            if (isEndOfFile)
                this.isEndOfFile = true;
        }

        public void WriteBytesToFile()
        {
            try
            {
                using (FileStream resultFileStream = outputFile.OpenWrite())
                {
                    while (true)
                    {

                        if (fileStreams.TryGetValue(currentWaitingBlock, out currentWaitingStream))
                        {
                            resultFileStream.Write(currentWaitingStream, 0, currentWaitingStream.Length);
                            fileStreams.TryRemove(currentWaitingBlock, out currentWaitingStream);
                            currentWaitingBlock++;
                            waitCounter = 0;
                        }

                        else
                        {
                            if (isEndOfFile && fileStreams.IsEmpty)
                            {
                                Console.WriteLine("\nFile has been decompressed.");
                                break;
                            }

                            Thread.Sleep(100);
                            waitCounter++;

                            if (waitCounter > 300)
                                throw new GZipTestException($"Decompress file error! Next required block (no. {currentWaitingBlock}) not found.");
                        }
                    }
                }
            }
            catch (GZipTestException e)
            {
                Console.WriteLine($"\n{e.Message}");
            }

            catch(Exception e)
            {
                Console.WriteLine($"\nEnexpected error! Stacktrace:\n{e.StackTrace}");
            }
        }
    }
}
