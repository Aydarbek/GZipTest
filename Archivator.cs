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
        const int partSize = 1048576; //5242880;

        static object locker = new object();

        internal static int threadsCount;

        int part = 1;
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
            threadsCount = 0;
            OutputStreamQueuer outputStreamQueuer = OutputStreamQueuer.GetInstance();
            outputStreamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(outputStreamQueuer.WriteStreamBytesToFile));
            compressThread.Start();

            for (long offset = 0; offset < sourceFile.Length; offset += partSize)
            {
                CompressionQueuer headeredFilePreparer = new CompressionQueuer(sourceFile, offset, part);
                Thread headerThread = new Thread(new ThreadStart(headeredFilePreparer.PrepareHeaderedFile));
                headerThread.Name = $"Thread_{part++}";
                headerThread.Start();
            }
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

        internal static void IncreaseThreadCount()
        {
            lock (locker)
            {
                threadsCount++;
            }

            Console.WriteLine($"\r{threadsCount} threads are working.");
        }

        internal static void DecreaseThreadCount()
        {
            lock (locker)
            {
                threadsCount--;
            }

            Console.WriteLine($"\r{threadsCount} threads are working.");
        }
    }
}
