using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeap
{
    /// <summary>
    /// File heap production implementation
    /// </summary>
    public class FileHeapDirectory : IFileHeapDirectory
    {
        public FileHeapDirectory(string BasePath)
        {
            this.BasePath = BasePath;
        }
        public readonly string BasePath;

        /// <summary>
        /// Gets the directory of the file in the heap in the following form:
        /// 2 bytes/2 bytes/2 bytes/2 bytes/56 bytes/
        /// </summary>
        /// <param name="Hash">The SHA256 hash of the file that needs to be stored or retrive</param>
        /// <returns></returns>
        public static string GetDirectory(byte[] Hash)
        {
            var Str = Cryptopack.Text.ByteArrayToHexString(Hash);
            var Directory =  Str.Substring(0, 2) + "\\" + Str.Substring(2, 2) + "\\" + Str.Substring(4, 2) + "\\" + Str.Substring(6, 2) + "\\" + Str.Substring(8, 56) + "\\";
            return Directory;
        }

        string IFileHeapDirectory.GetDirectory(byte[] Hash)
        {
            return System.IO.Path.Combine(BasePath, GetDirectory(Hash));
        }
    }
}
