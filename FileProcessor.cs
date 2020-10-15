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
    class FileProcessor
    {
        const int partSize = 1048576;  // 1 MegaByte in Bytes
        int part = 1;
        int totalBlocks = 0;
        int headerSize { get { return FileHeaderHandler.HEADER_SIZE; } }


        public void SplitAndZip(FileInfo fileToSplit)
        {            
            DateTime startTime = DateTime.Now;
            
            for (int offset = 0; offset < fileToSplit.Length; offset += partSize)
            {
                PartZipPreparer zipPreparer = new PartZipPreparer(fileToSplit, part,  offset);
                Thread zipThread = new Thread(new ThreadStart(zipPreparer.PreparePartZip));
                zipThread.Name = $"Thread_{part}";
                zipThread.Start();
                part++;
            }

            Console.WriteLine($"File was prepared in {(DateTime.Now - startTime).TotalSeconds} seconds");
            Console.ReadLine();
        }

        public void Split(FileInfo processingFile)
        {
            FileStream stream = File.Create($"{processingFile.Name}_headered");
            stream.Close();

            for (long offset = 0; offset < processingFile.Length; offset += partSize)
            {
                HeaderedFilePreparer headeredFilePreparer = new HeaderedFilePreparer(processingFile, offset, part);
                Thread headerThread = new Thread(new ThreadStart(headeredFilePreparer.PrepareHeaderedFile));
                headerThread.Name = $"Thread_{part++}";
                headerThread.Start();
            }
        }

        public void RestoreFile(FileInfo fileToRestore, FileInfo resultFile)
        {
            FileHeader currentHeader;

            using (FileStream fileToRestoreStream = fileToRestore.OpenRead())
            {
                FileHeader firstHeader = FileHeaderHandler.ReadFileHeader(fileToRestoreStream);
                FileRestorer fileRestorer = new FileRestorer(resultFile);
                Thread fileRetoreThread = new Thread(new ThreadStart(fileRestorer.WriteFileBytesToStream));
                fileRetoreThread.Start();
                
                byte[] firstFileByte = new byte[firstHeader.blockSize];
                fileToRestoreStream.Seek(headerSize, SeekOrigin.Begin);
                fileToRestoreStream.Read(firstFileByte, 0, firstFileByte.Length);
                fileRestorer.PutFileBytes(firstHeader.blockNum, firstFileByte, firstHeader.isEndOfFile);

                while (fileToRestoreStream.Position < fileToRestoreStream.Length)
                {
                    currentHeader = FileHeaderHandler.ReadFileHeader(fileToRestoreStream);
                    byte[] fileBytes = new byte[currentHeader.blockSize];
                    fileToRestoreStream.Read(fileBytes, 0, fileBytes.Length);
                    fileRestorer.PutFileBytes(currentHeader.blockNum, fileBytes, currentHeader.isEndOfFile);
                }

            }
        }



        //public void Merge(string fileName)
        //{            
        //    using (FileStream resultFile = File.Create(fileName))
        //    {
        //        FileInfo firstPart = new FileInfo($"{fileName}_part1");
        //        FileHeader firstFileHeader;
        //        FileHeader filePartHeader;

        //        using (FileStream firstFileStream = firstPart.OpenRead())
        //        {
        //            firstFileHeader = FileHeaderHandler.ReadFileHeader(firstFileStream);
        //            Console.WriteLine(firstFileHeader.ToString());
        //            byte[] firstFileByte = new byte[firstFileStream.Length - headerSize];
        //            firstFileStream.Seek(headerSize, SeekOrigin.Begin);
        //            firstFileStream.Read(firstFileByte, 0, firstFileByte.Length);
        //            resultFile.Write(firstFileByte, 0, firstFileByte.Length);
        //        }

        //        firstPart.Delete();

                
        //        for (int part = 2; part <= firstFileHeader.totalBlocks; part++)
        //        {
        //            FileInfo filePart = new FileInfo($"{fileName}_part{part}");

        //            using (FileStream filePartStream = filePart.OpenRead())
        //            {
        //                filePartHeader = FileHeaderHandler.ReadFileHeader(filePartStream);
        //                Console.WriteLine(filePartHeader.ToString());
        //                byte[] partFileByte = new byte[filePartStream.Length - headerSize];
        //                filePartStream.Seek(headerSize, SeekOrigin.Begin);
        //                filePartStream.Read(partFileByte, 0, partFileByte.Length);
        //                resultFile.Write(partFileByte, 0, partFileByte.Length);
        //            }
        //            filePart.Delete();
        //        }
        //    }
        //}

        private void CalculateTotalBlocks(long totalFileSize)
        {
            totalBlocks = (int)Math.Ceiling((double)totalFileSize / partSize);
        }

    }
}
