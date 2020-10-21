using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    interface IStreamQueuer
    {
        FileInfo outputFile { get; set; }
        Action ShowResult { get; set; }
        void PutBytesToQueue(int blockNum, byte[] bytes, bool isEndOfFile);
        void WriteBytesToFile();
    }
}
