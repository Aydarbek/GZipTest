using Microsoft.VisualBasic.Devices;
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
                ValidateInput(args);
                Archivator archivator = new Archivator(args[0]);
                archivator.ProcessFile(new FileInfo(args[1]), new FileInfo(args[2]));

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
            else if (args[1].Equals(args[2]))
                throw new GZipTestException("Input and output files cannot be the same!");
        }
    }
}
