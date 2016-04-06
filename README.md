# FileHeap
Hash based folder structure used for storing a large number of files

**What is the purpose of FileHeap?**
To store a large number of files in a folder structure each one identified with a 256-bit SHA2 hash of its content

**Folder structure:**
A file with a given hash is stored in:

`./aa/bb/cc/dd/eeeeee....ee/`

where `aa,bb,cc,dd` are the first 4 bytes of the hash and `eee...eee` are the last 28 bytes of the hash

This way any of the first 3 folders have at maximum 256 child folders, thus avoiding having a few folders each one with a large number of childrens.

Since the first 4 folders already manage to represent 256^4 combinations, the number of childs on the last depth on the tree is minimal, almost always one at a time, and slowly growing when needed

The file hash then can be stored in a database and be used to retrive or remove the files from the heap
