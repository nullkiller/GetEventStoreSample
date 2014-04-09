using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace EventStore.Infrastructure.Misc
{
    public class FileManager: IFileManager
    {
        public System.IO.TextReader OpenFile(string fileName)
        {
            fileName = GetRealName(fileName);
            return File.OpenText(fileName);
        }

        public System.IO.TextWriter CreateFile(string fileName)
        {
            fileName = GetRealName(fileName);
            return File.CreateText(fileName);
        }

        public void EnsureDirectory(string directory)
        {
            directory = GetRealName(directory);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public string[] GetFiles(string directory)
        {
            directory = GetRealName(directory);
            return Directory.GetFiles(directory);
        }

        private static string GetRealName(string fileName)
        {
            if (fileName.StartsWith("~"))
            {
                fileName = HttpContext.Current.Server.MapPath(fileName);
            }

            return fileName;
        }
    }
}
