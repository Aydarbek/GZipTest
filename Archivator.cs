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

        IFileReader fileReader;
        IStreamQueuer streamQueuer;
                

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
            fileReader = FileReader.GetInstance();
            streamQueuer = CompressionQueuer.GetInstance();

            try
            {
                if (!sourceFile.Exists)
                    throw new FileNotFoundException($"Specified file not found: {sourceFile.FullName}");
                
                startTime = DateTime.Now;

                StartOutputStreamQueuer(resultArchive);

                fileReader.sourceFile = sourceFile;

                for (int i = 1; i <= 10; i++)
                {
                    Thread blocksProcessingThread = new Thread(new ThreadStart(PrepareCompressedBlocks));
                    blocksProcessingThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (resultArchive.Exists)
                    resultArchive.Delete();
            }
        }

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            streamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(streamQueuer.WriteBytesToFile));
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

                zipBytes = GZipHelper.Compress(block.blockData);
                streamQueuer.PutBytesToQueue(block.blockNum, zipBytes, block.isEndOfFile);
            }           
        }

        internal void ShowTimeResult()
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine($"\nFile compressed in {elapsedTime.TotalSeconds:0.0} seconds");
        }

        public void DecompressFile(FileInfo fileToRestore, FileInfo resultFile)
        {
            fileReader = ZipReader.GetInstance();
            streamQueuer = DecompressionQueuer.GetInstance();

            try
            {
                if (!fileToRestore.Exists)
                    throw new FileNotFoundException($"Specified file not found: {fileToRestore.FullName}");

                ValidateFileFormat(fileToRestore);

                StartDecompressionQueuer(resultFile);
                fileReader.sourceFile = fileToRestore;

                for (int i = 1; i <= 5; i++)
                {
                    Thread readZipThread = new Thread(new ThreadStart(PrepareDecompressedBlocks));
                    readZipThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ValidateFileFormat(FileInfo fileToRestore)
        {
            using (FileStream fileStream = fileToRestore.OpenRead())
            {
                FileHeader fileHeader = FileHeaderHandler.ReadFileHeader(fileStream);
            }
        }

        private void StartDecompressionQueuer(FileInfo resultFile)
        {
            streamQueuer.outputFile = resultFile;
            Thread fileRestoreThread = new Thread(new ThreadStart(streamQueuer.WriteBytesToFile));
            fileRestoreThread.Start();
        }

        private void PrepareDecompressedBlocks()
        {
            while (true)
            {
                FileBlock fileBlock = fileReader.ReadNextBlock();

                if (fileBlock.isNull)
                    break;

                byte[] decompressedBytes = GZipHelper.Decompress(fileBlock.blockData);
                streamQueuer.PutBytesToQueue(fileBlock.blockNum, decompressedBytes, fileBlock.isEndOfFile);
            }
        }
    }
}