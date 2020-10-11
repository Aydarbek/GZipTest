using System.IO;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string command = args[0];
            FileInfo sourceFile = new FileInfo(args[1]);

            if ("compress".Equals(args[0]))
                GZipArchiver.Compress(sourceFile);
            else if ("decompress".Equals(args[0]))
                GZipArchiver.Decompress(sourceFile);
        }
    }
}
