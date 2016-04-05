using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeap
{
    /// <summary>
    /// Maps a file with a full path that is a function of its SHA-2 hash
    /// </summary>
    public interface IFileHeapDirectory
    {
        /// <summary>
        /// Gets a relative directory in the file heap of a file given its hash. File heap directories have always only one file at any time
        /// </summary>
        /// <param name="Hash">SHA2 hash of the file</param>
        string GetDirectory(byte[] Hash);
    }

    public static class FileHeapDirectoryExtensions
    {
        /// <summary>
        /// Gets the relative directory in the file heap of a file given its content. File heap directories have always only one file at any time
        /// </summary>
        /// <param name="FileData">Content of the file</param>
        /// <returns></returns>
        public static string GetDirectory(this IFileHeapDirectory Heap, Stream FileData)
        {
            var hash = Cryptopack.Hash.SHA2Bin(FileData);
            return Heap.GetDirectory(hash);
        }

        /// <summary>
        /// Gets the relative directory in the file heap of a file given its original path. File heap directories have always only one file at any time
        /// </summary>
        /// <param name="FilePath">The path of the file that will be stored on the heap</param>
        /// <returns></returns>
        public static string GetDirectory(this IFileHeapDirectory Heap, string FilePath)
        {
            byte[] hash;
            using (var F = new FileStream(FilePath, FileMode.Open))
            {
                hash = Cryptopack.Hash.SHA2Bin(F);
            }
            return Heap.GetDirectory(hash);
        }
    }
}
