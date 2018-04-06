using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhraseGenerator.Utils
{
    class FileHelper
    {
        public static string FileToString(string filename)
        {
            return File.ReadAllText(filename);
        }

        public static byte[] FileToBytes(string filename)
        {
            return File.ReadAllBytes(filename);
        }

    }
}
