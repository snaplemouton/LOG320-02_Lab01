using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Classes
{
    public class DictData : Dictionary<byte, string>
    {
        private string fileHeader;
        public string FileHeader
        {
            get
            {
                return fileHeader;
            }
            set
            {
                fileHeader = value;
            }
        }
    }

    public class CharData
    {
        private KeyValuePair<byte, int> pair;
        private string binaryCode;

        public CharData() { }
        public CharData(KeyValuePair<byte, int> pair) : this(pair, "") { }
        public CharData(KeyValuePair<byte, int> pair, string binaryCode)
        {
            this.pair = pair;
            this.binaryCode = binaryCode;
        }

        public KeyValuePair<byte, int> Pair
        {
            get
            {
                return pair;
            }
            set
            {
                pair = value;
            }
        }

        public string BinaryCode
        {
            get
            {
                return binaryCode;
            }
            set
            {
                binaryCode = value;
            }
        }
    }
}