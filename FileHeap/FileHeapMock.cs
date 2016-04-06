using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHeap
{
    /// <summary>
    /// In memory mock implementation of the file heap
    /// </summary>
    public class FileHeapMock : IFileHeap
    {
        class MockFile
        {
            public byte[] data { get; set; }
            public int users { get; set; }
        }
        private Dictionary<string, MockFile> Files = new Dictionary<string, MockFile>();
        /// <summary>
        /// Read an stream onto a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public void Add(Stream Data, byte[] Hash)
        {
            var th = Cryptopack.Text.ByteArrayToHexString(Hash);
            if (Files.ContainsKey(th))
            {
                Files[th].users++;
            }
            else
            {
                Files.Add(th, new MockFile
                {
                    data = ReadFully(Data),
                    users = 1
                });
            }
        }

        public string GetPath(byte[] Hash)
        {
            return Cryptopack.Text.ByteArrayToHexString(Hash);
        }

        public int GetUserCount(byte[] Hash)
        {
            var th = Cryptopack.Text.ByteArrayToHexString(Hash);
            MockFile F;
            if (Files.TryGetValue(th, out F))
            {
                return F.users;
            }
            else
                return 0;
        }

        public void Remove(byte[] Hash)
        {
            var th = Cryptopack.Text.ByteArrayToHexString(Hash);
            if (Files.ContainsKey(th))
            {
                var F = Files[th];
                if (F.users == 1)
                    Files.Remove(th);
                else
                    F.users--;
            }
            else
                throw new ArgumentException("File doesn't exists");
        }

        public Stream Read(byte[] Hash)
        {
            var th = Cryptopack.Text.ByteArrayToHexString(Hash);
            return new MemoryStream(Files[th].data);
        }
    }
}
