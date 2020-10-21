using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GZipTest.GZipHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GZipTest
{
    public class Archivator
    {
        DateTime startTime;

        IFileReader fileReader;
        IStreamQueuer streamQueuer;
        IGZipProcessor gZipProcessor;

        private static Archivator archivator;
        private Archivator() { }
        public static Archivator GetInstance()
        {
            if (archivator == null)
                archivator = new Archivator();

            return archivator;
        }
        

        public void ProcessFile(string operation, FileInfo sourceFile, FileInfo resultFile)
        {
            GetArchivatorTools(operation);

            try
            {
                if (!sourceFile.Exists)
                    throw new FileNotFoundException($"Specified file not found: {sourceFile.FullName}");
                
                startTime = DateTime.Now;

                StartOutputStreamQueuer(resultFile);
                fileReader.sourceFile = sourceFile;

                for (int i = 1; i <= 5; i++)
                {
                    Thread blocksProcessingThread = new Thread(new ThreadStart(PrepareProcessedBlocks));
                    blocksProcessingThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (resultFile.Exists)
                    resultFile.Delete();
            }
        }

        private void GetArchivatorTools(string operation)
        {
            fileReader = FileReaderFactory.GetFileReader(operation);
            streamQueuer = StreamQueuerFactory.GetStreamQueuer(operation);
            gZipProcessor = GZipProcessorFactory.GetGZipProcessor(operation);
        }


        //public void DecompressFile(FileInfo sourceFile, FileInfo resultFile)
        //{
        //    try
        //    {
        //        if (!sourceFile.Exists)
        //            throw new FileNotFoundException($"Specified file not found: {sourceFile.FullName}");

        //        //ValidateFileFormat(sourceFile);

        //        StartOutputStreamQueuer(resultFile);
        //        fileReader.sourceFile = sourceFile;

        //        for (int i = 1; i <= 5; i++)
        //        {
        //            Thread readZipThread = new Thread(new ThreadStart(PrepareProcessedBlocks));
        //            readZipThread.Start();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);

        //        if (resultFile.Exists)
        //            resultFile.Delete();
        //    }
        //}

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            streamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(streamQueuer.WriteBytesToFile));
            compressThread.Start();
        }


        private void PrepareProcessedBlocks()
        {
            FileBlock block;
            byte[] zipBytes;

            while (true)
            {
                block = fileReader.ReadNextBlock();

                if (block.isNull)
                    break;

                zipBytes = gZipProcessor.ProcessBytes(block.blockData);
                streamQueuer.PutBytesToQueue(block.blockNum, zipBytes, block.isEndOfFile);
            }           
        }

        internal void ShowTimeResult()
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine($"\nFile processed in {elapsedTime.TotalSeconds:0.0} seconds");
        }


        private void ValidateFileFormat(FileInfo fileToRestore)
        {
            using (FileStream fileStream = fileToRestore.OpenRead())
            {
                FileHeader fileHeader = FileHeaderHandler.ReadFileHeader(fileStream);
            }
        }

    }
}