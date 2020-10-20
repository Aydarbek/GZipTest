﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class ZipReader
    {
        static ReaderWriterLock rwl = new ReaderWriterLock();
        static object locker = new object();
        internal FileInfo sourceZipFile { get; set; }
        private long offset;
        FileHeader currentHeader;

        private static ZipReader zipReader;

        private ZipReader() { }

        internal static ZipReader GetInstance()
        {
            if (zipReader == null)
                zipReader = new ZipReader();

            return zipReader;
        }

        internal FileBlock ReadNextBlock()
        {
            lock (locker)
            {
                using (FileStream sourceZipFileStream = sourceZipFile.OpenRead())
                {
                    rwl.AcquireReaderLock(int.MaxValue);
                    sourceZipFileStream.Seek(offset, SeekOrigin.Begin);

                    if (sourceZipFileStream.Position == sourceZipFileStream.Length)
                        return new NullFileBlock();

                    currentHeader = FileHeaderHandler.ReadFileHeader(sourceZipFileStream);
                    byte[] zipBytes = new byte[currentHeader.blockSize];
                    sourceZipFileStream.Read(zipBytes, 0, zipBytes.Length);

                    offset += FileHeaderHandler.HEADER_SIZE + currentHeader.blockSize;

                    ShowCurrentStatus();

                    rwl.ReleaseReaderLock();

                    return new FileBlock(currentHeader.blockNum, zipBytes, currentHeader.isEndOfFile);
                }
            }
        }

        private void ShowCurrentStatus()
        {
            float progress = (float)offset / (float)sourceZipFile.Length * 100;
            Console.Write($"\rDecompressed {offset / 1024}/{sourceZipFile.Length / 1024} Kbytes ({progress:0.0}%)");
        }
    }
}