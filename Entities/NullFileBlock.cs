using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class NullFileBlock : FileBlock
    {
        public override int blockNum { get => -1; }
        internal override byte[] blockData { get => new byte[0]; }
        internal override bool isEndOfFile { get => true; }
        internal override bool isNull { get => true; }
    }
}
