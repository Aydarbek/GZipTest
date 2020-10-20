using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class FileBlock
    {
        public virtual int blockNum { get; }
        internal virtual byte[] blockData { get; }
        internal virtual bool isEndOfFile { get; }
        internal virtual bool isNull { get => false; }

        internal FileBlock() { }

        internal FileBlock(int blockNum, byte[] blockData, bool isEndOfFile)
        {
            this.blockNum = blockNum;
            this.blockData = blockData;
            this.isEndOfFile = isEndOfFile;
        }
    }
}
