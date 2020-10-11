using System;
using System.IO;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //string command = args[0];
            //FileInfo sourceFile = new FileInfo(args[1]);

            //if ("compress".Equals(args[0]))
            //    GZipArchiver.Compress(sourceFile);
            //else if ("decompress".Equals(args[0]))
            //    GZipArchiver.Decompress(sourceFile);
            //else if ("split".Equals(args[0]))
            //    FileSplitter.Split(sourceFile);

            string fileName = @"D:\Repos\GZipTest\bin\Debug\hu2000collector.log";
            FileInfo sourceFile = new FileInfo(fileName);
            if (sourceFile.Exists)
                FileSplitter.Split(sourceFile);
            else
                Console.WriteLine("File not found");
        }
    }
}
