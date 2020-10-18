using System;
using System.IO;

namespace GZipUnitTests
{
    class TestFixture : IDisposable
    {
        public TestFixture()
        {
            FileInfo resultArchive = new FileInfo(GZipIntegrationTest.projectDirectory +  @"\Files\postgresql-11.gzt");
            FileInfo resultFile = new FileInfo(GZipIntegrationTest.projectDirectory + @"\Files\kombinatorika.pdf");

            if(resultArchive.Exists)
                resultArchive.Delete();
            
            if(resultFile.Exists)
                resultFile.Delete();
        }

        public void Dispose()
        {
            
        }
    }
}
