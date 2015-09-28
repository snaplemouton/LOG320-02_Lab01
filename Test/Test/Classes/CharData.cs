using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Test.Classes
{
    public class BinaryTreeWithoutHeader<T> : BinaryTree<T>
    {
        private StringBuilder encodedText;

        public BinaryTreeWithoutHeader() : base() { }

        public StringBuilder EncodedText
        {
            get
            {
                return encodedText;
            }
            set
            {
                encodedText = value;
            }
        }
    }

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
        private byte charCode;

        public CharData() { }
        public CharData(KeyValuePair<byte, int> pair) : this(pair, "", new byte()) { }
        public CharData(KeyValuePair<byte, int> pair, string binaryCode) : this(pair, binaryCode, new byte()) { }
        public CharData(KeyValuePair<byte, int> pair, string binaryCode, byte charCode)
        {
            this.pair = pair;
            this.binaryCode = binaryCode;
            this.charCode = charCode;
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

        public byte CharCode
        {
            get
            {
                return charCode;
            }
            set
            {
                charCode = value;
            }
        }
    }
}