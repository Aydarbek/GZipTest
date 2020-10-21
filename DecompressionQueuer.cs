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
    class DecompressionQueuer
    {
        ConcurrentDictionary<int, byte[]> fileStreams = new ConcurrentDictionary<int, byte[]>();
        internal FileInfo resultFile { get; set; }
        bool isEndOfFile { get; set; }
        int currentWaitingBlock = 1;
        byte[] currentWaitingStream;
        int waitCounter = 0;

        private static DecompressionQueuer decompressionQueuer;

        private DecompressionQueuer()  {}


        internal static DecompressionQueuer GetInstance()
        {
            if (decompressionQueuer == null)
                decompressionQueuer = new DecompressionQueuer();

            return decompressionQueuer;
        }

        internal void PutFileBytes(int blockNum, byte[] fileStream, bool isEndOfFile)
        {
            fileStreams.TryAdd(blockNum, fileStream);
            if (isEndOfFile)
                this.isEndOfFile = true;
        }

        internal void WriteFileBytesToStream()
        {
            try
            {
                using (FileStream resultFileStream = resultFile.OpenWrite())
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
