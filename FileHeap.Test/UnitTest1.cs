using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace FileHeap.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PathTest()
        {
            var FileData = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            var Heap = new FileHeapDirectory("C:\\tmp\\heap");
            var Path = Heap.GetDirectory(new MemoryStream(FileData));

            Assert.AreEqual(@"C:\tmp\heap\32\bb\e3\78\a25091502b2baf9f7258c19444e7a43ee4593b08030acd790bd66e6a\", Path);
        }

        private static MemoryStream GetData(string Data)
        {
            return new MemoryStream(System.Text.Encoding.ASCII.GetBytes(Data));
        }

        [TestMethod]
        public void HeapTest()
        {
            //Create a temporal directory in C:\
            var TestFolder = @"C:\FileHeap test EB67BDA058D5\tmp 1\tmp 2\tmp 3\tmp 4\";

            if (Directory.Exists(TestFolder))
            {
                Directory.Delete(TestFolder, true);
            }

            Directory.CreateDirectory(TestFolder);
            var HeapDir = new FileHeapDirectory(TestFolder);
            IFileHeap Heap = new FileHeap(HeapDir, new FileSystem());

            var Hash1 = Heap.Add(GetData("Prueba"));
            //Check that the hash is returned correctly
            Assert.AreEqual(Cryptopack.Hash.SHA2(System.Text.Encoding.ASCII.GetBytes("Prueba")), Cryptopack.Text.ByteArrayToHexString(Hash1));

            var Hash2 = Heap.Add(GetData("Hola a todos"));

            //Check that both files exists:
            Assert.IsTrue(File.Exists(Heap.GetPath(Hash1)));
            Assert.IsTrue(File.Exists(Heap.GetPath(Hash2)));

            //Check that the content of both files are correct
            Assert.AreEqual(System.IO.File.ReadAllText(Heap.GetPath(Hash1)), "Prueba");
            Assert.AreEqual(System.IO.File.ReadAllText(Heap.GetPath(Hash2)), "Hola a todos");

            //Add the same content multiple times:
            Heap.Add(GetData("Prueba"));
            Heap.Add(GetData("Prueba"));

            //Check that the number of users of that data is correct:
            Assert.AreEqual(3, Heap.GetUserCount(Hash1));
            Assert.AreEqual(1, Heap.GetUserCount(Hash2));

            Assert.AreEqual(2, Directory.GetDirectories(TestFolder).Length);
            //Remove the second object, the whole folder structure for that file should be removed:
            Heap.Remove(Hash2);
            Assert.AreEqual(1, Directory.GetDirectories(TestFolder).Length);

            //Add a second object with a hash almost the same as the first object:
            var Hash3 = Hash1.ToArray();
            unchecked
            {
                Hash3[Hash3.Length - 1]++;
            }

            Heap.Add(GetData("Prueba 2"), Hash3);

            //No directories have been added
            Assert.AreEqual(1, Directory.GetDirectories(TestFolder).Length);

            //Check user count:
            Assert.AreEqual(3, Heap.GetUserCount(Hash1));
            Assert.AreEqual(1, Heap.GetUserCount(Hash3));

            var Hash31Folder = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(Heap.GetPath(Hash1)));

            //Two folders are on ac 3f 09 1c directory, each one containing Hash1 and Hash3 files:
            Assert.AreEqual(2, Directory.GetDirectories(Hash31Folder).Length);

            //Remove hash 3:
            Heap.Remove(Hash3);

            //One folders are on ac 3f 09 1c directory, containing Hash1 files
            Assert.AreEqual(1, Directory.GetDirectories(Hash31Folder).Length);

            Heap.Remove(Hash1);
            Heap.Remove(Hash1);

            //One folders are on ac 3f 09 1c directory, containing Hash1 files
            Assert.AreEqual(1, Directory.GetDirectories(Hash31Folder).Length);
            Assert.AreEqual(1, Heap.GetUserCount(Hash1));

            Heap.Remove(Hash1);

            Assert.AreEqual(0, Heap.GetUserCount(Hash1));

            Assert.IsFalse(Directory.Exists(Hash31Folder));

            //Clean up
            if (Directory.Exists(TestFolder))
            {
                Directory.Delete(@"C:\FileHeap test EB67BDA058D5", true);
            }
        }
    }
}
