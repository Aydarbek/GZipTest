using Microsoft.VisualBasic.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GZipTest
{
    class Program
    {
        const string argumentExceptionText = "Incorrect command";

        static int Main(string[] args)
        {
            try
            {
                if (args != null)
                    ValidateInput(args);

                if ("compress".Equals(args[0]))
                    Archivator.GetInstance().CompressFile(new FileInfo(args[1]), new FileInfo(args[2]));

                else if ("decompress".Equals(args[0]))
                    Archivator.GetInstance().DecompressFile(new FileInfo(args[1]), new FileInfo(args[2]));

                else
                    throw new ArgumentException(argumentExceptionText);

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
            
        }
    }
}
