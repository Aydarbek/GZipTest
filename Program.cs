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
            //fileProcessor.Split(fileName);
            fileProcessor.Merge(@"D:\Repos\GZipTest\bin\Debug\getAllCrossConnections.xml");

            Console.ReadLine();


            //string command;
            //FileInfo sourceFile;

            //FileProcessor fileProcessor = new FileProcessor();
            //if (args.Length > 0)
            //{
            //    command = args[0];
            //    sourceFile = new FileInfo(args[1]);

            //    if ("compress".Equals(args[0]))
            //        GZipArchiver.Compress(sourceFile);
            //    else if ("decompress".Equals(args[0]))
            //        GZipArchiver.Decompress(sourceFile);
            //    else if ("split".Equals(args[0]))
            //        fileProcessor.Split(sourceFile);
            //}            

            //else
            //{

            //    //string fileName = @"D:\Repos\GZipTest\bin\Debug\getAllEquipment.xml";
            //    //FileInfo sourceFile = new FileInfo(fileName);
            //    //if (sourceFile.Exists)
            //    //    fileProcessor.Split(sourceFile);
            //    //else
            //    //    Console.WriteLine("File not found");

            //    fileProcessor.Merge("getAllEquipment.xml");
            //}
        }

       
    }
}
