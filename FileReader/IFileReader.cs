using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    interface IFileReader
    {
        FileInfo sourceFile { get; set; }
        FileBlock ReadNextBlock();
    }
}
