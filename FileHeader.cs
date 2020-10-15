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
        public int totalBlocks { get; set; }
        public int blockSize { get; set; }

        public FileHeader(int blockNum, int totalBlocks, int blockSize)
        {
            this.blockNum = blockNum;
            this.totalBlocks = totalBlocks;
            this.blockSize = blockSize;
        }

        public override string ToString()
        {
            return $"Block {blockNum} from {totalBlocks}. Block size {blockSize} bytes";
        }
    }
}
