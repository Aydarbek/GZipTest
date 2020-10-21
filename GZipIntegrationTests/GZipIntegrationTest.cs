using System;
using Xunit;
using GZipTest;
using System.IO;
using System.Threading;
using System.Reflection;

namespace GZipUnitTests
{
    public class GZipIntegrationTest : IClassFixture<TestFixture>
    {
        internal static string workingDirectory = Directory.GetCurrentDirectory();
        internal static string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

        [Fact]
        public void CompressFileTest()
        {
            Archivator archivator = new Archivator("compress");
            FileInfo sourceFile = new FileInfo(projectDirectory + @"\Files\postgresql-11.pdf");
            FileInfo resultArchive = new FileInfo(projectDirectory + @"\Files\postgresql-11.gzt");

            archivator.ProcessFile(sourceFile, resultArchive);
            Thread.Sleep(2000);

            Assert.True(resultArchive.Exists);
            Assert.Equal(7631223, resultArchive.Length);
        }

        [Fact]
        public void DecompressFileTest()
        {
            Archivator archivator = new Archivator("decompress");
            FileInfo sourceArchive = new FileInfo(projectDirectory + @"\Files\kombinatorika.gzt");
            FileInfo resultFile = new FileInfo(projectDirectory + @"\Files\kombinatorika.pdf");

            archivator.ProcessFile(sourceArchive, resultFile);
            Thread.Sleep(1000);

            Assert.True(resultFile.Exists);
            Assert.Equal(9620347, resultFile.Length);
        }
    }
}
