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
        string operation;

        public Archivator(string operation)
        {
            this.operation = operation;
        }
        

        public void ProcessFile(FileInfo sourceFile, FileInfo resultFile)
        {
            startTime = DateTime.Now;

            try
            {
                GetArchivatorTools(operation);
                ValidateSourceFile(operation, sourceFile);
                StartOutputStreamQueuer(resultFile);
                StartFileProcessingThreads(sourceFile);
            }
            catch (Exception)
            {
                if (resultFile.Exists)
                    resultFile.Delete();

                throw;
            }
        }

        private void GetArchivatorTools(string operation)
        {
            fileReader = FileReaderFactory.GetFileReader(operation);
            streamQueuer = StreamQueuerFactory.GetStreamQueuer(operation);
            streamQueuer.ShowResult = ShowOperationResult;

            gZipProcessor = GZipProcessorFactory.GetGZipProcessor(operation);
        }

        private void StartFileProcessingThreads(FileInfo sourceFile)
        {
            fileReader.sourceFile = sourceFile;

            for (int i = 1; i <= 5; i++)
            {
                Thread blocksProcessingThread = new Thread(new ThreadStart(PrepareProcessedBlocks));
                blocksProcessingThread.Start();
            }
        }

        private void ValidateSourceFile(string operation, FileInfo sourceFile)
        {
            if (!sourceFile.Exists)
                throw new FileNotFoundException($"Specified file not found: {sourceFile.FullName}");

            if ("decompress".Equals(operation))
                ValidateFileFormat(sourceFile);
        }

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            streamQueuer.outputFile = resultArchive;
            Thread compressThread = new Thread(new ThreadStart(streamQueuer.WriteBytesToFile));
            compressThread.Start();
        }

        private void PrepareProcessedBlocks()
        {
            FileBlock block;
            byte[] processedBytes;

            while (true)
            {
                block = fileReader.ReadNextBlock();

                if (block.isNull)
                    break;

                processedBytes = gZipProcessor.ProcessBytes(block.blockData);
                streamQueuer.PutBytesToQueue(block.blockNum, processedBytes, block.isEndOfFile);
            }           
        }

        internal void ShowOperationResult()
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine($"\nFile {operation}ed in {elapsedTime.TotalSeconds:0.0} seconds");
        }


        private void ValidateFileFormat(FileInfo fileToRestore)
        {
            using (FileStream fileStream = fileToRestore.OpenRead())
            {
                FileHeader fileHeader = FileHeaderHelper.ReadFileHeader(fileStream);
            }
        }
    }
}