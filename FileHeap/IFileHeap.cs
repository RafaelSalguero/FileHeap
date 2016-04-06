using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeap
{
    /// <summary>
    /// Store and retrive files from a hash based file heap
    /// </summary>
    public interface IFileHeap
    {
        /// <summary>
        /// If the file doesn't exist on the heap, set the user count to 1 and saves it to the heap, if the file already exists, only increment the user count. Returns the hash of the file
        /// </summary>
        /// <param name="Data">The data of the file</param>
        /// <param name="Hash">The hash of the file</param>
        void Add(Stream Data, byte[] Hash);

        /// <summary>
        /// If the file doesn't exist on the heap, an exception is thrown, if the user count is 1, the file is deleted and the folders are removed if empty, if the user count is greater than 1, just decrement its
        /// </summary>
        /// <param name="Hash">The hash of the file to remove</param>
        void Remove(byte[] Hash);

        /// <summary>
        /// Gets the user count of a file, which is the number of times the same file has been added
        /// </summary>
        /// <returns></returns>
        int GetUserCount(byte[] Hash);

        /// <summary>
        /// Gets the path of the file containing the data of the given hash
        /// </summary>
        /// <param name="Hash">SHA2 hash of the data</param>
        string GetPath(byte[] Hash);

        /// <summary>
        /// Read a file given its hash
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        Stream Read(byte[] Hash);
    }

    public static class FileHeapExtensions
    {
        /// <summary>
        /// If the file doesn't exist on the heap, set the user count to 1 and saves it to the heap, if the file already exists, only increment the user count. Returns the hash of the file. This method loads the entire file to memory in order to get the hash of the data.
        /// Returns the hash of the data
        /// </summary>
        /// <param name="Heap"></param>
        /// <param name="Data">The data of the file</param>
        /// <returns>The SHA256 hash of the data</returns>
        public static byte[] Add(this IFileHeap Heap, Stream Data)
        {
            var M = new MemoryStream();
            Data.CopyTo(M);
            M.Position = 0;
            var Hash = Cryptopack.Hash.SHA2Bin(M);
            M.Position = 0;
            Heap.Add(M, Hash);

            return Hash;
        }
    }

    /// <summary>
    /// Implementation of the IFileHeap that uses an abstraction of the file system. File data is stored on a "data.bin" file on the directory folder and the user count on the "users.txt"
    /// </summary>
    public class FileHeap : IFileHeap
    {
        /// <summary>
        /// Create a new file heap
        /// </summary>
        /// <param name="DirectorySolver">The class that provides paths to file directories given its hash</param>
        /// <param name="FileSystem">Abstraction of the file system that removes all dependencies of the real file system by the FileHeap</param>
        public FileHeap(IFileHeapDirectory DirectorySolver, IFileSystem FileSystem)
        {
            this.DirectorySolver = DirectorySolver;
            this.FileSystem = FileSystem;
        }
        readonly IFileHeapDirectory DirectorySolver;
        readonly IFileSystem FileSystem;

        const string DataFileName = "data.bin";
        const string CountFileName = "users.txt";
        const int DirectoryDepth = 5;

        public string GetPath(byte[] Hash)
        {
            var dir = DirectorySolver.GetDirectory(Hash);
            var dataPath = System.IO.Path.Combine(dir, DataFileName);
            return dataPath;
        }

        public void Add(Stream Data, byte[] Hash)
        {
            var UserCount = GetUserCount(Hash);
            var dir = DirectorySolver.GetDirectory(Hash);
            var countPath = System.IO.Path.Combine(dir, CountFileName);
            if (UserCount == 0)
            {
                //The file doesn't exists, it data and users file needs to be created
                var dataPath = System.IO.Path.Combine(dir, DataFileName);

                FileSystem.CreateDirectory(dir);

                //The data is copied
                using (var DataStream = FileSystem.WriteFile(dataPath))
                {
                    Data.CopyTo(DataStream);
                    DataStream.Flush();
                }

                //User count is created with 1
                using (var CountStream = FileSystem.WriteFile(countPath))
                {
                    var Writer = new StreamWriter(CountStream);
                    Writer.Write("1");
                    Writer.Flush();
                    CountStream.Flush();
                }
            }
            else
            {
                //Increment user count
                using (var CountStream = FileSystem.WriteFile(countPath))
                {
                    var Writer = new StreamWriter(CountStream);
                    Writer.Write(UserCount + 1);
                    Writer.Flush();
                    CountStream.Flush();
                }
            }
        }

        public void Remove(byte[] Hash)
        {
            var UserCount = GetUserCount(Hash);
            var dir = DirectorySolver.GetDirectory(Hash);
            var countPath = System.IO.Path.Combine(dir, CountFileName);
            var dataPath = System.IO.Path.Combine(dir, DataFileName);

            if (UserCount == 0)
            {
                throw new ArgumentException("File doesn't exists on heap");
            }
            else if (UserCount == 1)
            {
                //Remove the data, user and the directory;
                FileSystem.RemoveFile(countPath);
                FileSystem.RemoveFile(dataPath);

                try
                {
                    var dirToRemove = System.IO.Path.GetDirectoryName(dataPath);
                    for (var i = 0; i < DirectoryDepth; i++)
                    {
                        FileSystem.RemoveDirectory(dirToRemove);
                        dirToRemove = System.IO.Path.GetDirectoryName(dirToRemove);
                    }
                }
                catch (Exception)
                {
                    //The directory wasn't empty
                }
            }
            else
            {
                //Decrement user count
                using (var CountStream = FileSystem.WriteFile(countPath))
                {
                    var Writer = new StreamWriter(CountStream);
                    Writer.Write(UserCount - 1);
                    Writer.Flush();
                    CountStream.Flush();
                }
            }
        }

        public int GetUserCount(byte[] Hash)
        {
            var dir = this.DirectorySolver.GetDirectory(Hash);
            var countPath = System.IO.Path.Combine(dir, CountFileName);
            if (FileSystem.DirectoryExists(dir) && FileSystem.FileExists(countPath))
            {
                using (var Stream = FileSystem.OpenFile(countPath))
                {
                    var Reader = new StreamReader(Stream);
                    var Str = Reader.ReadToEnd();
                    return int.Parse(Str);
                }
            }
            return 0;
        }

        public Stream Read(byte[] Hash)
        {
            return new FileStream(GetPath(Hash), FileMode.Open);
        }
    }
}
