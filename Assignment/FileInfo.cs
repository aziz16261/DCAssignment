using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    public class FileStore
    {
        public string fileName {  get; set; }

        public byte[] fileData { get; set; }

        public FileStore(string iFileName, byte[] iFileData) {
            fileName = iFileName;
            fileData = iFileData;
        }
    }
}
