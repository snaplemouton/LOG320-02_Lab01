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
            DictData binaryDictByte = BuildBinaryDict(binaryTree);
            byte[] newLstByte = BuildNewByteArray(lstByte, binaryDictByte);

            // Save file on server and force download for the user.
            string fileName = file.FileName + ".yolo";
            File.WriteAllBytes(HttpContext.Current.Server.MapPath(fileName), newLstByte);
            HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.AddHeader("Content-Disposition",
                               "attachment; filename=" + fileName + ";");
            response.WriteFile(HttpContext.Current.Server.MapPath(fileName));
            response.End();
        }

        /// <summary>
        /// Method used to decompress the previously compressed file and save it into its previous format.
        /// </summary>
        /// <param name="file">File to decompress.</param>
        public void DecompressFile(HttpPostedFile file)
        {
            byte[] lstByte = null;
            BinaryReader binaryReader = new BinaryReader(file.InputStream);
            lstByte = binaryReader.ReadBytes(file.ContentLength);

            // Rebuild the Binary Tree that will be used to provide the original Byte Array
            BinaryTreeWithoutHeader<CharData> binaryTree = RebuildBinaryTree(lstByte);
            // Rebuild the original byte list using the binary tree and encoded text found in the BinaryTreeWithoutHeader object.
            byte[] originalLstByte = RebuildOriginalLstByte(binaryTree);

            // Save file on server and force download for the user.
            string fileName = file.FileName.Substring(0, file.FileName.LastIndexOf('.'));
            File.WriteAllBytes(HttpContext.Current.Server.MapPath(fileName), originalLstByte);
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
        protected DictData BuildBinaryDict(BinaryTree<CharData> binaryTree)
        {
            string headerTree = "";
            // The header is used to rebuild the binary tree when decompressing.
            // Example...
            //      o
            //    /  \
            //   o    o
            //  / \  / \
            // o  o  o  o
            //     \   / \
            //      o  o  o
            // This binary tree would be made like this (If right = 01, left = 10 and parent = 11. 00 would mean the end of the binary tree creation)
            // 01 01 01 11 10 11 11 10 11 11 10 01 01 11 11 10 11 11 11 00

            string currentBinaryCode = "";
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
                    headerTree += "11";
                    // Is a Leaf
                    if (!isParent)
                    {
                        currentNode.Value.BinaryCode = currentBinaryCode;
                        headerTree += Convert.ToString(Convert.ToByte(currentNode.Value.Pair.Key), 2).PadLeft(8, '0');
                        dictTmp.Add(currentNode.Value.Pair.Key, currentNode.Value.BinaryCode);
                    }
                    currentNode = currentNode.Parent;
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
            dictTmp.FileHeader = headerTree;
            return dictTmp;
        }

        /// <summary>
        /// Build the final byte array of the compressed file.
        /// </summary>
        /// <param name="oldByteArray">The original byte array used to change the bytes into the new bytes.</param>
        /// <param name="binaryDictByte">The Dictionary containing the information used to compare the old bytes.</param>
        /// <returns>The new byte array used for the compressed file.</returns>
        protected byte[] BuildNewByteArray(byte[] oldByteArray, DictData binaryDictByte)
        {
            int i = 0;
            // StringBuilder used to negate the speed lost from adding to a very long string.
            StringBuilder sb = new StringBuilder();
            sb.Append(binaryDictByte.FileHeader);
            // Go through the old byte array to create the new bytes.
            while (i < oldByteArray.Length)
            {
                sb.Append(binaryDictByte[oldByteArray[i]]);
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
        #endregion

        #region Rebuild methods
        /// <summary>
        /// Build a BinaryTreeWithoutHeader object that extend the BinaryTree class.
        /// This object contains the encoded text as well as the binary tree.
        /// </summary>
        /// <param name="lstByte">The byte list from the compressed file.</param>
        /// <returns>The built BinaryTreeWithoutHeader object.</returns>
        protected BinaryTreeWithoutHeader<CharData> RebuildBinaryTree(byte[] lstByte)
        {
            BinaryTreeNode<CharData> currentNode = new BinaryTreeNode<CharData>();
            BinaryTreeWithoutHeader<CharData> binaryTree = new BinaryTreeWithoutHeader<CharData>();
            binaryTree.Root = currentNode;

            StringBuilder sb = new StringBuilder();
            foreach (byte b in lstByte)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            bool headerDone = false;
            bool isCharCode = false;
            bool isRight = true;
            bool charCodeWasJustDone = false;
            int index = 0;
            string currentChunk = "";
            string currentBinaryCode = "";
            string previousBinaryCode = "";
            while (!headerDone)
            {
                if (!isCharCode && currentChunk.Length == 2)
                {
                    // Do something with the tree
                    switch (currentChunk)
                    {
                        case "01":
                            // Going right
                            currentBinaryCode += "1";
                            charCodeWasJustDone = false;
                            currentNode.Right = new BinaryTreeNode<CharData>();
                            currentNode.Right.Parent = currentNode;
                            currentNode = currentNode.Right;
                            isRight = true;
                            break;
                        case "10":
                            // Going left
                            currentBinaryCode += "0";
                            charCodeWasJustDone = false;
                            currentNode.Left = new BinaryTreeNode<CharData>();
                            currentNode.Left.Parent = currentNode;
                            currentNode = currentNode.Left;
                            isRight = false;
                            break;
                        case "11":
                            // Going parent
                            if (!charCodeWasJustDone)
                            {
                                isCharCode = true;
                            }
                            if (currentBinaryCode.Length > 0)
                            {
                                previousBinaryCode = currentBinaryCode;
                                currentBinaryCode = currentBinaryCode.Substring(0, currentBinaryCode.Length - 1);
                            }
                            currentNode = currentNode.Parent;
                            break;
                        case "00":
                            // Binary tree done
                            if(charCodeWasJustDone)
                                headerDone = true;
                            break;
                    }
                    currentChunk = "";
                }
                else if (isCharCode && currentChunk.Length == 8)
                {
                    CharData nodeToAdd = new CharData(new KeyValuePair<byte, int>(), previousBinaryCode, Convert.ToByte(currentChunk, 2));
                    // You got a char
                    if (isRight)
                    {
                        currentNode.Right.Value = nodeToAdd;
                    }
                    else
                    {
                        currentNode.Left.Value = nodeToAdd;
                    }
                    isCharCode = false;
                    charCodeWasJustDone = true;
                    currentChunk = "";
                }
                if (!headerDone)
                {
                    currentChunk += sb[index];
                    index++;
                }
            }
            sb.Remove(0, index);
            binaryTree.EncodedText = sb;
            return binaryTree;
        }

        /// <summary>
        /// Rebuild the byte list from the original uncompressed file using the BinaryTreeWithoutHeader object made from the RebuildBinaryTree method.
        /// </summary>
        /// <param name="binaryTree">The BinaryTreeWithoutHeader object to use.</param>
        /// <returns>The byte list from the original uncompressed file.</returns>
        protected byte[] RebuildOriginalLstByte(BinaryTreeWithoutHeader<CharData> binaryTree)
        {
            List<byte> originalLstByteTmp = new List<byte>();
            BinaryTreeNode<CharData> currentNode = binaryTree.Root;
            String str = binaryTree.EncodedText.ToString(0, binaryTree.EncodedText.Length);
            int i = 0;
            while (i < binaryTree.EncodedText.Length)
            {
                if (str[i] == '0')
                {
                    // Move Left
                    if (currentNode.Left != null)
                    {
                        currentNode = currentNode.Left;
                    }
                    else
                    {
                        originalLstByteTmp.Add(currentNode.Value.CharCode);
                        currentNode = binaryTree.Root.Left;
                    }
                }
                else
                {
                    // Move Right
                    if (currentNode.Right != null)
                    {
                        currentNode = currentNode.Right;
                    }
                    else
                    {
                        originalLstByteTmp.Add(currentNode.Value.CharCode);
                        currentNode = binaryTree.Root.Right;
                    }
                }
                i++;
            }
            originalLstByteTmp.Add(currentNode.Value.CharCode);
            byte[] originalByteArray = originalLstByteTmp.ToArray();
            return originalByteArray;
        }
        #endregion
    }
}