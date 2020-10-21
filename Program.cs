﻿using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GZipTest
{
    class Program
    {
        const string argumentExceptionText = 
            "Incorrect parameters\n" +
            "Usage: GZipTest.exe compress/decompress SOURCE_FILE DESTINATION_FILE";

        static int Main(string[] args)
        {
            try
            {
                //Archivator.GetInstance().CompressFile(new FileInfo("Europe.log"), new FileInfo("result.gzt"));
                //Archivator.GetInstance().DecompressFile(new FileInfo("result.gzt"), new FileInfo("Europe1.log"));

                ValidateInput(args);

                Archivator.GetInstance().ProcessFile(args[0], new FileInfo(args[1]), new FileInfo(args[2]));

                return 0;
        }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
        }

        private static void ValidateInput(string [] args)
        {
            if (args == null || 
                args.Length < 3 ||
                (!("compress".Equals(args[0])) && !("decompress".Equals(args[0]))))
                    throw new ArgumentException(argumentExceptionText);
        }
    }
}
