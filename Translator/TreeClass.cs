using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Translator
{
    public class TreeClass
    {
        private Node _root;

        public Node Root { get => _root; set => _root = value; }

        public Node AddNode(string inputDataNode, Node root)
        {
            if (root == null)
            {
                root = new Node(inputDataNode);
            }
            else
            {
                //добавление в дерево, выбор правого/левого поддерева
                if ( (inputDataNode != "+") && (inputDataNode != "-") && (inputDataNode != "*") && (inputDataNode != "/") )
                {
                    root.Left = AddNode(inputDataNode, root.Left);
                }
                else
                {
                    root.Right = AddNode(inputDataNode, root.Right);
                }
            }

            return root;
        }

        /*public Node FindElement(int findData, Node root)
        {
            if (root == null || findData == root.Data)
                return root;
            else if (root.Data < findData)
                return FindElement(findData, root.Left);
            else
                return FindElement(findData, root.Right);
        }*/

       /* public Node Minimum(Node root)
        {
            if (root != null)
            {
                if (root.Left != null) root = Minimum(root.Left);
            }
            return root;
        }*/

       /* public Node DeleteNode(int deleteData, Node root)
        {
            if (root == null)
                return root;
            if (deleteData < root.Data)
            {
                root.Left = DeleteNode(deleteData, root.Left);
            }
            else if (deleteData > root.Data)
            {
                root.Right = DeleteNode(deleteData, root.Right);
            }
            else if (root.Left != null && root.Right != null)
            {
                root.Data = Minimum(root.Right).Data;
                root.Right = DeleteNode(root.Data, root.Right);
            }
            else if (root.Left != null)
            {
                return root.Left;
            }
            else
            {
                return root.Right;
            }

            return root;

        }*/

        public void PrintTree(int x, int y, Node root, int delta = 0)
        {
            if (root != null)
            {
                if (delta == 0) delta = x / 2;
                Console.SetCursorPosition(x, y);
                Console.Write(root.Data);
                PrintTree(x - delta, y + 3, root.Left, delta / 2);
                PrintTree(x + delta, y + 3, root.Right, delta / 2);
            }

        }

        public void ClearTree()
        {
            _root = null;
        }

        public int CountElements(Node root)
        {
            if (root == null)
                return 0;
            else
            {
                int count = 0;
                count += CountElements(root.Left);
                count += CountElements(root.Right);

                return count + 1;
            }
        }

        public bool IsEmpty()
        {
            return _root == null ? true : false;
        }

    }

    public class Node
    {
        private string _data;
        private Node _left;
        private Node _right;

        public Node()
        {
        }

        public Node(string inputDataNode)
        {
            Data = inputDataNode;
        }

        public Node(string data, Node left, Node right)
        {
            Data = data;
            Left = left;
            Right = right;
        }

        public string Data { get => _data; set => _data = value; }
        public Node Left { get => _left; set => _left = value; }
        public Node Right { get => _right; set => _right = value; }
    }
}