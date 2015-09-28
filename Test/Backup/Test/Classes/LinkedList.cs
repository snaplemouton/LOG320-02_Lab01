using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test.Classes
{
    // Linked list
    // firstNode: Represent the first node in the list
    // lastNode: Optional - Represent the last node in the list
    public class LinkedList<T>
    {
        private LinkedListNode<T> firstNode;
        private LinkedListNode<T> lastNode;
        private int length;

        public LinkedList() { }
        public LinkedList(LinkedListNode<T> firstNode)
        {
            this.firstNode = firstNode;
            this.lastNode = firstNode;
            this.length = 1;
        }

        public LinkedListNode<T> FirstNode
        {
            get
            {
                return firstNode;
            }
            set
            {
                firstNode = value;
            }
        }

        public LinkedListNode<T> LastNode
        {
            get
            {
                return lastNode;
            }
            set
            {
                lastNode = value;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public void RemoveFirstNode()
        {
            firstNode = firstNode.Next;
            length--;
        }
    }

    // Node for the linked list
    public class LinkedListNode<T>
    {
        private T data;
        private LinkedListNode<T> next;
        private LinkedListNode<T> previous;
        
        public LinkedListNode() { }
        public LinkedListNode(T data) : this(data, null, null) { }
        public LinkedListNode(T data, LinkedListNode<T> next) : this(data, next, null) { }
        public LinkedListNode(T data, LinkedListNode<T> next, LinkedListNode<T> previous)
        {
            this.data = data;
            this.next = next;
            this.previous = previous;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public LinkedListNode<T> Next
        {
            get
            {
                return next;
            }
            set
            {
                next = value;
            }
        }

        public LinkedListNode<T> Previous
        {
            get
            {
                return previous;
            }
            set
            {
                previous = value;
            }
        }
    }
}