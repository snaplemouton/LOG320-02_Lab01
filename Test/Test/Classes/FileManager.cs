using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace Test.Classes
{
    public class FileManager
    {
        #region Compress and Decompress method
        /// <summary>
        /// Method used to compress the file and save it.
        /// Using Hoffman's Binary Tree.
        /// </summary>
        /// <param name="file">File to compress.</param>
        public void CompressFile(HttpPostedFile file)
        {
            // Read the file and fill the byte list
            byte[] lstByte = null;
            BinaryReader binaryReader = new BinaryReader(file.InputStream);
            lstByte = binaryReader.ReadBytes(file.ContentLength);

            // Build the Dictionary of bytes and their occurence in the list.
            Dictionary<byte, int> dictByte = BuildDictionary(lstByte);

            // Build the Linked List needed for creating the Binary Tree using the Dictionary built previously
            BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> lnkLstByte = BuildLinkedList(dictByte);

            // Build the Binary Tree that will be used to provide the new Byte Array
            BinaryTree<CharData> binaryTree = BuildBinaryTree(lnkLstByte);

            // Build the new Byte Array
            //BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> binaryLnkLstByte = BuildBinaryLinkedList(binaryTree);
            DictData binaryDictByte = BuildBinaryDict(binaryTree);
            byte[] newLstByte = BuildNewByteArray(lstByte, binaryDictByte);
            //byte[] newLstByte = BuildNewByteArray(lstByte, binaryLnkLstByte);
            string fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.')) + ".yolo";
            File.WriteAllBytes(HttpContext.Current.Server.MapPath(fileName), newLstByte);
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + fileName + ";");
            response.WriteFile(HttpContext.Current.Server.MapPath(fileName));
            response.End();
        }
        #endregion

        #region Build methods
        /// <summary>
        /// Build the dictionary containing each bytes and their number of occurences in the byte list.
        /// </summary>
        /// <param name="lstByte">Byte list to transform into a dictionary.</param>
        /// <returns>The dictionary containing each bytes and their number of occurences.</returns>
        protected Dictionary<Byte, int> BuildDictionary(Byte[] lstByte)
        {
            Dictionary<byte, int> dictByte = new Dictionary<byte, int>();
            int index = 0;
            while (index < lstByte.Length)
            {
                if (dictByte.ContainsKey(lstByte[index]))
                {
                    dictByte[lstByte[index]]++;
                }
                else
                {
                    dictByte.Add(lstByte[index], 1);
                }
                index++;
            }
            return dictByte;
        }

        /// <summary>
        /// Build the linked list containing each dictionary pair in an ascending order.
        /// </summary>
        /// <param name="dictByte">Dictionary of bytes and their number of occurences to use.</param>
        /// <returns>The linked list containing the dictionary pair in an ascending order.</returns>
        protected BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> BuildLinkedList(Dictionary<byte, int> dictByte)
        {
            BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> lnkLstByte = new BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>>();
            // Go through the dictionary to create the Linked List
            foreach (KeyValuePair<byte, int> pair in dictByte){
                lnkLstByte.InsertIntoList(new LinkedListNode<BinaryTreeNode<CharData>>(new BinaryTreeNode<CharData>(new CharData(pair))), true);
            }
            return lnkLstByte;
        }

        /// <summary>
        /// Build the binary tree that will be used to create the new byte array.
        /// </summary>
        /// <param name="lnkLstByte">The linked list that will be used to build the binary tree.</param>
        /// <returns>The binary tree that will be used for creating the new byte array.</returns>
        protected BinaryTree<CharData> BuildBinaryTree(BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> lnkLstByte)
        {
            BinaryTreeNode<CharData> currentLeftNode = null;
            BinaryTreeNode<CharData> currentRightNode = null;
            BinaryTreeNode<CharData> combinedNode = null;
            BinaryTree<CharData> binaryTree = new BinaryTree<CharData>();

            while (lnkLstByte.FirstNode.Next != null)
            {
                currentRightNode = lnkLstByte.FirstNode.Value;
                lnkLstByte.RemoveFirstNode();
                currentLeftNode = lnkLstByte.FirstNode.Value;
                lnkLstByte.RemoveFirstNode();
                combinedNode = new BinaryTreeNode<CharData>(new CharData(new KeyValuePair<byte, int>(new byte(), currentRightNode.Value.Pair.Value + currentLeftNode.Value.Pair.Value)), currentLeftNode, currentRightNode);
                currentLeftNode.Parent = combinedNode;
                currentRightNode.Parent = combinedNode;
                lnkLstByte.InsertIntoList(new LinkedListNode<BinaryTreeNode<CharData>>(combinedNode), true);
            }
            binaryTree.Root = lnkLstByte.FirstNode.Value;
            return binaryTree;
        }

        /// <summary>
        /// Build the linked list containing each dictionary pair with their new binary code in a descending order.
        /// </summary>
        /// <param name="binaryTree">Binary tree to use.</param>
        /// <returns>The linked list containing the dictionary pair and their binary code in a descending order.</returns>
        protected DictData BuildBinaryDict(BinaryTree<CharData> binaryTree)//BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> BuildBinaryLinkedList(BinaryTree<CharData> binaryTree)
        {
            string headerTree = "";
            string headerChars = "";
            // The header is used to rebuild the binary tree when decompressing.
            // Example...
            //      o
            //    /  \
            //   o    o
            //  / \  / \
            // o  o  o  o
            //     \   / \
            //      o  o  o
            // This binary tree would be made like this (If right = 01, left = 10 and parent = 11. 00 would mean the start and the end of the binary tree creation)
            // 00 01 01 01 11 10 11 11 10 11 11 10 01 01 11 11 10 00

            string currentBinaryCode = "";
            //BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> lnkLstTemp = new BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>>();
            DictData dictTmp = new DictData();
            BinaryTreeNode<CharData> currentNode = binaryTree.Root;
            bool isParent = false;
            while (currentNode != null)
            {
                if (currentNode.Right != null)
                {
                    // Has a right node
                    currentNode = currentNode.Right;
                    headerTree += "01";
                    currentBinaryCode += "1";
                    isParent = false;
                }
                else if (currentNode.Left != null)
                {
                    // Has a left node but not right
                    currentNode = currentNode.Left;
                    headerTree += "10";
                    currentBinaryCode += "0";
                    isParent = false;
                }
                else
                {
                    // Is a Leaf
                    if (!isParent)
                    {
                        currentNode.Value.BinaryCode = currentBinaryCode;
                        headerChars += currentBinaryCode + Convert.ToString(Convert.ToByte(currentNode.Value.Pair.Key), 2);
                        dictTmp.Add(currentNode.Value.Pair.Key, currentNode.Value.BinaryCode);
                        //lnkLstTemp.InsertIntoList(new LinkedListNode<BinaryTreeNode<CharData>>(currentNode), false);
                    }
                    currentNode = currentNode.Parent;
                    headerTree += "11";
                    isParent = true;
                    if (currentNode != null)
                    {
                        currentBinaryCode = currentBinaryCode.Remove(currentBinaryCode.Length - 1);
                        if (currentNode.Right != null)
                            currentNode.Right = null;
                        else currentNode.Left = null;
                    }
                }
            }
            headerTree += "00";
            dictTmp.FileHeader = headerTree + headerChars;
            return dictTmp;
            //lnkLstTemp.FileHeader = headerTree + headerChars;
            //return lnkLstTemp;
        }

        /// <summary>
        /// Build the final byte array of the compressed file.
        /// </summary>
        /// <param name="oldByteArray">The original byte array used to change the bytes into the new bytes.</param>
        /// <param name="binaryLnkLstByte">The linked list containing the information used to compare the old bytes.</param>
        /// <returns>The new byte array used for the compressed file.</returns>
        protected byte[] BuildNewByteArray(byte[] oldByteArray, DictData binaryDictByte) //BinaryTreeNodeLinkedList<BinaryTreeNode<CharData>> binaryLnkLstByte)
        {
            int i = 0;
            // StringBuilder used to negate the speed lost from adding to a very long string.
            StringBuilder sb = new StringBuilder();
            sb.Append(binaryDictByte.FileHeader);
            // Go through the old byte array to create the new bytes.
            while (i < oldByteArray.Length)
            {
                sb.Append(binaryDictByte[oldByteArray[i]]);
                //currentByteString += FindByteInBinaryLinkedList(oldByteArray[i], binaryLnkLstByte.FirstNode);
                i++;
            }
            string currentByteString = sb.ToString();
            int numOfExtraBits = currentByteString.Length % 8;
            if(numOfExtraBits > 0)
                currentByteString = currentByteString.PadLeft(currentByteString.Length + 8 - numOfExtraBits, '0');
            int numOfBytes = currentByteString.Length / 8;
            byte[] newByteArray = new byte[numOfBytes];
            for (int ii = 0; ii < numOfBytes; ii++)
            {
                newByteArray[ii] = Convert.ToByte(currentByteString.Substring(8 * ii, 8), 2);
            }
            return newByteArray;
        }

        protected string FindByteInBinaryLinkedList(byte b, LinkedListNode<BinaryTreeNode<CharData>> n)
        {
            while (n != null)
            {
                if (n.Value.Value.Pair.Key == b)
                    return n.Value.Value.BinaryCode;
                n = n.Next;
            }
            return "";
        }
        #endregion

        /// <summary>
        /// Method used to decompress the previously compressed file and save it into its previous format.
        /// </summary>
        /// <param name="file">File to decompress.</param>
        public void DecompressFile(FileStream file)
        {
            // Do something
        }
    }
}