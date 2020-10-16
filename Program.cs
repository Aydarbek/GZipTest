using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GZipTest
{
    class Program
    {
        static void Main(string[] args)
        {
            FileProcessor fileProcessor = new FileProcessor();
            FileInfo fileName = new FileInfo (@"D:\Repos\GZipTest\bin\Debug\getAllCrossConnections.xml");
            //fileProcessor.CompressFile(fileName);
            fileProcessor.DecompressFile(new FileInfo(@"D:\Repos\GZipTest\bin\Debug\test.txt_headered"),
                new FileInfo(@"D:\Repos\GZipTest\bin\Debug\test.txt_restored"));

            Console.ReadLine();
        }
    }
}
