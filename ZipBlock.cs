using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    struct ZipBlock
    {
        internal int blockNum;
        internal byte[] blockData;
        internal bool isEndOfFile;

        internal ZipBlock(int blockNum, byte[] blockData, bool isEndOfFile)
        {
            this.blockNum = blockNum;
            this.blockData = blockData;
            this.isEndOfFile = isEndOfFile;
        }
    }
}
