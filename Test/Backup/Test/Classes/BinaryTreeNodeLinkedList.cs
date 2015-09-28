using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Classes
{
    public class BinaryTreeNodeLinkedList<T> : LinkedList<BinaryTreeNode<CharData>>
    {
        private string fileHeader = "";
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

        public void InsertIntoList(LinkedListNode<BinaryTreeNode<CharData>> newLnkLstNode, bool asc)
        {
            // Check if we are at the first node
            if (this.FirstNode != null)
            {
                // Check if smaller/equal or greater than First Node
                if ((newLnkLstNode.Value.Value.Pair.Value <= this.FirstNode.Value.Value.Pair.Value && asc) ||
                    (newLnkLstNode.Value.Value.Pair.Value > this.FirstNode.Value.Value.Pair.Value && !asc))
                {
                    // If yes, insert new node at the start of the Linked List.
                    this.FirstNode.Previous = newLnkLstNode;
                    newLnkLstNode.Next = this.FirstNode;
                    this.FirstNode = newLnkLstNode;
                }
                else
                {
                    if (this.FirstNode.Next != null)
                    {
                        LinkedListNode<BinaryTreeNode<CharData>> lnkLstNode = this.FirstNode.Next;
                        // Go through the Linked List to find where to place the new node.
                        while (lnkLstNode != null)
                        {
                            // Check if smaller/equal or greater than the current Node.
                            if ((newLnkLstNode.Value.Value.Pair.Value <= lnkLstNode.Value.Value.Pair.Value && asc) ||
                                (newLnkLstNode.Value.Value.Pair.Value > lnkLstNode.Value.Value.Pair.Value && !asc))
                            {
                                // If yes, insert new node at the previous next position and break the while loop.
                                lnkLstNode.Previous.Next = newLnkLstNode;
                                newLnkLstNode.Previous = lnkLstNode.Previous;
                                lnkLstNode.Previous = newLnkLstNode;
                                newLnkLstNode.Next = lnkLstNode;
                                break;
                            }
                            // Else, check if last node of the Linked List is null.
                            if (lnkLstNode.Next == null)
                            {
                                // If yes, add to the end and break the while loop.
                                newLnkLstNode.Previous = lnkLstNode;
                                lnkLstNode.Next = newLnkLstNode;
                                this.LastNode = newLnkLstNode;
                                break;
                            }
                            // Else, continue the while loop.
                            lnkLstNode = lnkLstNode.Next;
                        }
                    }
                    else
                    {
                        newLnkLstNode.Previous = this.FirstNode;
                        this.FirstNode.Next = newLnkLstNode;
                        this.LastNode = newLnkLstNode;
                    }
                }
            }
            else
            {
                this.FirstNode = newLnkLstNode;
                this.LastNode = newLnkLstNode;
            }
            this.Length++;
        }
    }
}