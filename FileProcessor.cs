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
        int headerSize { get { return FileHeaderHandler.HEADER_SIZE; } }


        public void CompressFile(FileInfo processingFile)
        {
            FileInfo compressedFile = new FileInfo($"{processingFile.Name}_headered");
            OutputStreamQueuer outputStreamQueuer = OutputStreamQueuer.GetInstance();
            outputStreamQueuer.outputFile = compressedFile;
            Thread compressThread = new Thread(new ThreadStart(outputStreamQueuer.WriteStreamBytesToFile));
            compressThread.Start();

            for (long offset = 0; offset < processingFile.Length; offset += partSize)
            {
                HeaderedFilePreparer headeredFilePreparer = new HeaderedFilePreparer(processingFile, offset, part);
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
                FileRestorer fileRestorer = new FileRestorer(resultFile);
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
    }
}
