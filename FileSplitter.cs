using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    class FileSplitter
    {
        const int partSize = 1048576;  // 1 MegaByte in Bytes

        public static void Split(FileInfo fileToSplit)
        {
            int part = 1;
            int position = 0;

            for (int offset = 0; offset < fileToSplit.Length; offset += partSize)
            {
                using (FileStream fileStream = fileToSplit.OpenRead())
                {
                    byte[] bytes = new byte[Math.Min(partSize, fileToSplit.Length - offset)];

                    fileStream.Seek(offset, SeekOrigin.Begin);
                    fileStream.Read(bytes, 0, bytes.Length);

                    using (FileStream partFile = File.Create(fileToSplit.Name + ".part" + part++))
                    {
                        partFile.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
    }
}
