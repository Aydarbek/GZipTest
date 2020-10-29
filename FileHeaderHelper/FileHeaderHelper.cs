using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GZipTest
{
    class FileHeaderHelper
    {
        internal const int HEADER_SIZE = 64;

        internal static void WriteFileHeader(FileStream stream, FileHeader fileHeader)
        {
            try
            {
                byte[] headerStream = new byte[HEADER_SIZE];

                string headerInfo = JsonConvert.SerializeObject(fileHeader);
                byte[] headerInfoBytes = Encoding.Default.GetBytes(headerInfo);
                Array.Copy(headerInfoBytes, headerStream, headerInfoBytes.Length);

                stream.Write(headerStream, 0, HEADER_SIZE);
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static FileHeader ReadFileHeader(FileStream stream)
        {
            try
            {
                byte[] byteArray = new byte[HEADER_SIZE];
                stream.Read(byteArray, 0, HEADER_SIZE);

                string fileHeaderString = Encoding.Default.GetString(byteArray).Trim();

                return JsonConvert.DeserializeObject<FileHeader>(fileHeaderString);
            }
            catch (JsonReaderException)
            {
                throw new GZipTestException("Archive file format is not GZipTest format or archive file corrupted.");
            }
        }
    }
}
