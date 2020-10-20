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
        DateTime startTime;

        FileReader fileReader = FileReader.GetInstance();
        ZipReader zipReader = ZipReader.GetInstance();

        CompressionQueuer outputStreamQueuer = CompressionQueuer.GetInstance();
        DecompressionQueuer decompressionQueuer = DecompressionQueuer.GetInstance();

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
            startTime = DateTime.Now;

            StartOutputStreamQueuer(resultArchive);

            fileReader.sourceFile = sourceFile;

            for (int i = 1; i <= 10; i++)
            {
                Thread blocksProcessingThread = new Thread(new ThreadStart(PrepareCompressedBlocks));
                blocksProcessingThread.Start();
            }
        }

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            outputStreamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(outputStreamQueuer.WriteStreamBytesToFile));
            compressThread.Start();
        }

        private void PrepareCompressedBlocks()
        {
            FileBlock block;
            byte[] zipBytes;

            while (true)
            {
                block = fileReader.ReadNextBlock();

                if (block.isNull)
                    break;

                zipBytes = GZipArchiver.Compress(block.blockData);
                CompressionQueuer.GetInstance().WriteBytesToQueue(block.blockNum, zipBytes, block.isEndOfFile);
            }
        }

        internal void ShowTimeResult()
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine($"\nFile compressed in {elapsedTime.TotalSeconds:0.0} seconds");
        }

        public void DecompressFile(FileInfo fileToRestore, FileInfo resultFile)
        {
            try
            {
                StartDecompressionQueuer(resultFile);

                zipReader.sourceZipFile = fileToRestore;

                for (int i = 1; i <= 10; i++)
                {
                    Thread readZipThread = new Thread(new ThreadStart(PrepareDecompressedBlocks));
                    readZipThread.Start();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void StartDecompressionQueuer(FileInfo resultFile)
        {
            decompressionQueuer.resultFile = resultFile;
            Thread fileRestoreThread = new Thread(new ThreadStart(decompressionQueuer.WriteFileBytesToStream));
            fileRestoreThread.Start();
        }

        private void PrepareDecompressedBlocks()
        {
            while (true)
            {
                FileBlock fileBlock = zipReader.ReadNextBlock();

                if (fileBlock.isNull)
                    break;

                byte[] decompressedBytes = GZipArchiver.Decompress(fileBlock.blockData);
                decompressionQueuer.PutFileBytes(fileBlock.blockNum, decompressedBytes, fileBlock.isEndOfFile);
            }
        }
    }
}
