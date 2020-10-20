using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GZipTest
{
    class CompressionQueuer
    {
        FileReader fileReader = FileReader.GetInstance();
        FileBlock block;
        byte[] zipBytes;

        internal void PrepareCompressedBlock()
        {
            while (true)
            {
                block = fileReader.ReadNextBlock();

                if(block.blockData.Length == 0)
                    break;

                FileHeader fileHeader = new FileHeader(block.blockNum, block.blockData.Length, block.isEndOfFile);

                zipBytes = GZipArchiver.Compress(block.blockData);
                OutputStreamQueuer.GetInstance().WriteBytesToQueue(block.blockNum, zipBytes, block.isEndOfFile);
            }
        }
    }
}
