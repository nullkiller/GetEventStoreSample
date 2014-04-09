using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.Infrastructure.Misc
{
    public interface IFileManager
    {
        TextReader OpenFile(string fileName);
        TextWriter CreateFile(string fileName);

        void EnsureDirectory(string directory);

        string[] GetFiles(string directory);
    }
}
