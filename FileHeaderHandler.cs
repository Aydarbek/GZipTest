using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GZipTest
{
    class FileHeaderHandler
    {
        internal const int HEADER_SIZE = 128;

        internal static void WriteFileHeader(FileStream stream, FileHeader fileHeader)
        {
            byte[] headerStream = new byte[HEADER_SIZE];

            string headerInfo = JsonConvert.SerializeObject(fileHeader);
            byte[] headerInfoBytes = Encoding.Default.GetBytes(headerInfo);

            for (int i = 0; i < headerInfoBytes.Length; i++)
                headerStream[i] = headerInfoBytes[i];

            stream.Write(headerStream, 0, HEADER_SIZE);
            Console.WriteLine($"Header size {headerInfoBytes.Length}");
        }

        internal static FileHeader ReadFileHeader(FileStream stream)
        {
            byte[] byteArray = new byte[HEADER_SIZE];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(byteArray, 0, HEADER_SIZE);

            string fileHeaderString = Encoding.Default.GetString(byteArray).Trim();

            return JsonConvert.DeserializeObject<FileHeader>(fileHeaderString);
        }
    }
}
