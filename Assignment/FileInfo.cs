using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    public class FileStore
    {
        [DataMember]
        public string fileName {  get; set; }
        [DataMember]
        public byte[] fileData { get; set; }

        public FileStore(string iFileName, byte[] iFileData) {
            fileName = iFileName;
            fileData = iFileData;
        }
    }
}
