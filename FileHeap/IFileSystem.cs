using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeap
{
    /// <summary>
    /// Interface for filesystem operations in order to enable mocking
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Checks if a directory exists
        /// </summary>
        /// <param name="Directory">Directory path to check</param>
        /// <returns></returns>
        bool DirectoryExists(string Directory);
        /// <summary>
        /// Checks if a file exists
        /// </summary>
        /// <param name="Path">File path to check</param>
        /// <returns></returns>
        bool FileExists(string Path);

        /// <summary>
        /// Create the given directory
        /// </summary>
        /// <param name="Directory"></param>
        void CreateDirectory(string Directory);

        /// <summary>
        /// Remove the directory. Throws an exception if the directory is not empty
        /// </summary>
        /// <param name="Directory"></param>
        void RemoveDirectory(string Directory);

        /// <summary>
        /// Removes a file
        /// </summary>
        /// <param name="Path"></param>
        void RemoveFile(string Path);

        /// <summary>
        /// Open a reading stream on a file
        /// </summary>
        /// <param name="Path">The path of the file</param>
        /// <returns></returns>
        Stream OpenFile(string Path);

        /// <summary>
        /// Open a write stream on a given path
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        Stream WriteFile(string Path);
    }

    /// <summary>
    /// Production file system
    /// </summary>
    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string Directory)
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        public bool DirectoryExists(string Directory)
        {
            return System.IO.Directory.Exists(Directory);
        }

        public bool FileExists(string Path)
        {
            return System.IO.File.Exists(Path);
        }

        public Stream OpenFile(string Path)
        {
            return new FileStream(Path, FileMode.Open);
        }

       
        public void RemoveDirectory(string Directory)
        {
            System.IO.Directory.Delete(Directory);
        }

        public void RemoveFile(string Path)
        {
            System.IO.File.Delete(Path);
        }

        public Stream WriteFile(string Path)
        {
            return new FileStream(Path, FileMode.Create);
        }
    }

}
