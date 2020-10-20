using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    struct FileBlock
    {
        internal int blockNum;
        internal byte[] blockData;
        internal bool isEndOfFile;

        internal FileBlock(int blockNum, byte[] blockData, bool isEndOfFile)
        {
            this.blockNum = blockNum;
            this.blockData = blockData;
            this.isEndOfFile = isEndOfFile;
        }
    }
}
