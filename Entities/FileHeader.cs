using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    internal struct FileHeader
    {
        public int blockNum { get; set; }
        public int blockSize { get; set; }
        public bool isEndOfFile { get; set; }

        public FileHeader(int blockNum, int blockSize, bool isEndOfFile)
        {
            this.blockNum = blockNum;
            this.blockSize = blockSize;
            this.isEndOfFile = isEndOfFile;
        }

        public override string ToString()
        {
            return $"Block {blockNum}. Block size {blockSize} bytes. Is last block: {isEndOfFile}";
        }
    }
}
