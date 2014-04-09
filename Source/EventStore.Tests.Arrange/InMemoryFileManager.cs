using EventStore.Infrastructure.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Tests.Arrange
{
    public class InMemoryFileManager: IFileManager
    {
        public StringBuilder Data { get; private set; }
        public string FileName { get; set; }

        public InMemoryFileManager()
        {
            Data = new StringBuilder();
        }

        public TextReader OpenFile(string fileName)
        {
            FileName = fileName;
            return new StringReader(Data.ToString());
        }

        public TextWriter CreateFile(string fileName)
        {
            FileName = fileName;
            return new StringWriter(Data);
        }

        public void EnsureDirectory(string directory)
        {
        }

        public string[] GetFiles(string directory)
        {
            return new string[] { Path.Combine(directory, "snapshot0.json"), FileName };
        }
    }
}
