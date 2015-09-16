using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Test.Classes
{
    public class FileManager
    {
        public void CompressFile(FileStream file)
        {
            byte[] lstByte = new byte[file.Length];
            if(file.CanRead)
            {
                file.Read(lstByte, 0, (int)file.Length);
            }
        }

        public void DecompressFile(FileStream file)
        {

        }
    }
}