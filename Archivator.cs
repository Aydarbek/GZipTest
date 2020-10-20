using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GZipTest
{
    public class Archivator
    {
        static object locker = new object();

        internal static int threadsCount = 0;

        int headerSize { get { return FileHeaderHandler.HEADER_SIZE; } }
        private static Archivator archivator;

        private Archivator() { }

        public static Archivator GetInstance()
        {
            if (archivator == null)
                archivator = new Archivator();

            return archivator;
        }

        public void CompressFile(FileInfo sourceFile, FileInfo resultArchive)
        {
            StartOutputStreamQueuer(resultArchive);

            FileReader.GetInstance().sourceFile = sourceFile;

            for (int i = 1; i <= 10; i++)
            {
                CompressionQueuer compressionQueuer = new CompressionQueuer();
                Thread blocksProcessingThread = new Thread(new ThreadStart(compressionQueuer.PrepareCompressedBlock));
                blocksProcessingThread.Name = $"Thread_{i}";
                blocksProcessingThread.Start();
            }
        }

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            OutputStreamQueuer outputStreamQueuer = OutputStreamQueuer.GetInstance();
            outputStreamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(outputStreamQueuer.WriteStreamBytesToFile));
            compressThread.Start();
        }

        public void DecompressFile(FileInfo fileToRestore, FileInfo resultFile)
        {
            FileHeader currentHeader;

            using (FileStream fileToRestoreStream = fileToRestore.OpenRead())
            {
                DecompressionQueuer fileRestorer = new DecompressionQueuer(resultFile);
                Thread fileRestoreThread = new Thread(new ThreadStart(fileRestorer.WriteFileBytesToStream));
                fileRestoreThread.Start();

                while (fileToRestoreStream.Position < fileToRestoreStream.Length)
                {
                    currentHeader = FileHeaderHandler.ReadFileHeader(fileToRestoreStream);
                    byte[] fileBytes = new byte[currentHeader.blockSize];
                    fileToRestoreStream.Read(fileBytes, 0, fileBytes.Length);

                    byte[] decompressedBytes = GZipArchiver.Decompress(fileBytes);
                    fileRestorer.PutFileBytes(currentHeader.blockNum, decompressedBytes, currentHeader.isEndOfFile);
                }
            }
        }

        //internal static void IncreaseThreadCount()
        //{
        //    lock (locker)
        //    {
        //        threadsCount++;
        //    }

        //    Console.WriteLine($"\r{threadsCount} threads are working.");
        //}

        //internal static void DecreaseThreadCount()
        //{
        //    lock (locker)
        //    {
        //        threadsCount--;
        //    }

        //    Console.WriteLine($"\r{threadsCount} threads are working.");
        //}
    }
}
