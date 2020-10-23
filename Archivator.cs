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
        IFileReader fileReader;
        IStreamQueuer streamQueuer;
        IGZipProcessor gZipProcessor;

        Thread fileOutputThread;
        internal static Exception threadException;

        string operation;
        long maxThreadsCount;
        DateTime startTime;

        public Archivator(string operation)
        {
            this.operation = operation;
        }

        public void ProcessFile(FileInfo sourceFile, FileInfo resultFile)
        {
            try
            {
                startTime = DateTime.Now;
                SetUpThreadsLimit(sourceFile);
                GetArchivatorTools(operation);
                ValidateSourceFile(operation, sourceFile);
                StartOutputStreamQueuer(resultFile);
                StartFileProcessingThreads(sourceFile);

                fileOutputThread.Join();

                if (threadException != null)
                    throw threadException;

                ShowOperationResult();
            }
            catch (Exception)
            {
                if (resultFile.Exists)
                    resultFile.Delete();

                throw;
            }
        }

        private void SetUpThreadsLimit(FileInfo sourceFile)
        {
            if ("compress".Equals(operation)) 
                maxThreadsCount = 10;
            else if ("decompress".Equals(operation))
                maxThreadsCount = 5;
        }

        private void GetArchivatorTools(string operation)
        {
            fileReader = FileReaderFactory.GetFileReader(operation);
            streamQueuer = StreamQueuerFactory.GetStreamQueuer(operation);
            gZipProcessor = GZipProcessorFactory.GetGZipProcessor(operation);
        }

        private void StartOutputStreamQueuer(FileInfo resultArchive)
        {
            streamQueuer.outputFile = resultArchive;
            fileOutputThread = new Thread(new ThreadStart(streamQueuer.WriteBytesToFile));
            fileOutputThread.Start();
        }

        private void StartFileProcessingThreads(FileInfo sourceFile)
        {
            fileReader.sourceFile = sourceFile;

            for (int i = 1; i <= maxThreadsCount; i++)
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

        private void PrepareProcessedBlocks()
        {
            try
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
            catch (Exception ex)
            {
                threadException = ex;
            }         
        }


        private void ShowOperationResult()
        {
            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine($"\nFile {operation}ed in {elapsedTime.TotalSeconds:0.0} seconds");
        }


        private void ValidateFileFormat(FileInfo fileToRestore)
        {
            try
            {
                using (FileStream fileStream = fileToRestore.OpenRead())
                {
                    FileHeader fileHeader = FileHeaderHelper.ReadFileHeader(fileStream);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}