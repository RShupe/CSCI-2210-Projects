﻿using System;
using System.Collections.Generic;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	File Name:         BTree.cs
//	Description:       This class creates and manages a BTree
//
//	Course:            CSCI 2210 - Data Structures
//	Author:            Ryan Shupe, shuper@etsu.edu, East Tennessee State University
//	Created:           Wednesday Apr 17, 2019
//
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Project_5
{
    internal class BTree
    {
        public int count { get; set; }
        public int indexCount { get; set; }
        public int leafCount { get; set; }
        public int nodeSize { get; set; }
        public Node root { get; set; }
        public Stack<Node> stack { get; set; }
        public bool Trace { get; set; }

        #region Constructors

        /// <summary>
        /// Basic no argument constructor
        /// </summary>
        public BTree()
        {
            count = 0;
            indexCount = 0;
            nodeSize = 0;
        }

        /// <summary>
        /// Constructor that takes in an integer
        /// </summary>
        /// <param name="n">the node side to be set </param>
        public BTree(int n)
        {
            indexCount = 0;
            count = 0;
            nodeSize = n;

            stack = new Stack<Node>();
            root = new Index(nodeSize);
            Leaf leaf = new Leaf(nodeSize);

            ((Index)root).Insert(0, leaf);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// This method adds a value to the BTree
        /// </summary>
        /// <param name="n">The value that will be added to the tree.</param>
        /// <returns>bool that says if the operation is successful</returns>
        public bool AddValue(int n)
        {
            bool added = false; ;

            Leaf leaf = findLeaf(n);
            INSERT insert = leaf.Insert(n);

            if (insert == INSERT.DUPLICATE)
            {
                added = false;
            }
            else
            {
                count++;
                if (insert != INSERT.SUCCESS)
                {
                    splitLeaf(leaf, n);
                    added = true;
                }
                else
                {
                    stack.Pop();
                    Index index = (Index)stack.Peek();

                    int temp = 0;

                    while (added == false)
                    {
                        if (temp >= index.Indexes.Count)
                        {
                            added = true;
                        }
                        else if (index.Indexes[temp] == leaf)
                        {
                            index.value[temp] = leaf.value[0];
                        }

                        temp++;
                    }
                }
            }
            return added;
        }

        /// <summary>
        /// This method displays the entire tree.
        /// </summary>
        public void Display()
        {
            leafCount = 0;
            indexCount = 0;

            Console.WriteLine("Root node:");
            Display(root, 0);
            Console.WriteLine("\n\n" + Stats());
        }

        /// <summary>
        /// This method displays a node
        /// </summary>
        /// <param name="node">node to be displayed</param>
        /// <param name="inNum"></param>
        public void Display(Node node, int inNum)
        {
            Console.WriteLine(node);

            if (node is Index)
            {
                inNum++;
                Console.WriteLine("Level in the BTree: " + inNum);
                indexCount++;

                for (int i = 0; i < ((Index)node).Indexes.Count; i++)
                {
                    Display(((Index)node).Indexes[i], inNum);
                }
            }
            else
            {
                leafCount++;
            }
        }

        /// <summary>
        /// returns an integer of the depth
        /// </summary>
        /// <returns>the depth</returns>

        public int findDepth()
        {
            int n = 0;
            Node nodeRoot = root;
            while (nodeRoot is Index)
            {
                nodeRoot = ((Index)nodeRoot).Indexes[0];
                n++;
            }

            return n;
        }

        /// <summary>
        /// This method finds an appropriate leaf and returns the leaf object.
        /// </summary>
        /// <param name="inNumLeaf">leaf to look for</param>
        /// <returns>the returning leaf node.</returns>
        public Leaf findLeaf(int inNumLeaf)
        {
            Node searchNode = root;
            stack.Clear();

            while (searchNode is Index)
            {
                if (Trace)
                {
                    Console.WriteLine(searchNode);
                }

                stack.Push(searchNode);
                Index indexNode = (Index)searchNode;

                int i;
                for (i = 1; i < indexNode.value.Count && inNumLeaf >= indexNode.value[i]; i++)
                {
                }

                searchNode = indexNode.Indexes[i - 1];
            }

            stack.Push(searchNode);

            if (Trace)
            {
                Console.WriteLine(searchNode);
            }

            return (Leaf)searchNode;
        }

        /// <summary>
        /// This method checks to see if a value is in the tree
        /// </summary>
        /// <param name="n">value to find</param>
        /// <returns>returning result if it is found or not.</returns>
        public bool findValue(int n)
        {
            Trace = true;
            int result = findLeaf(n).value.IndexOf(n);
            Trace = false;

            bool equal = (indexCount != -1);

            return equal;
        }

        /// <summary>
        /// Appropriately sorts.
        /// </summary>
        /// <param name="inListInt"></param>
        /// <param name="inNodeList"></param>
        private void Sort(List<int> inListInt, List<Node> inListNode)
        {
            bool sorted = false;
            int n = 0;
            while (inListInt.Count > n++ && !sorted)
            {
                for (int i = 0; i < inListInt.Count - n; i++)
                {
                    if (inListInt[i] > inListInt[i + 1])
                    {
                        int x = inListInt[i];
                        inListInt[i] = inListInt[i + 1];
                        inListInt[i + 1] = x;

                        Node tempNode = inListNode[i];


                        inListNode[i] = inListNode[i + 1];
                        inListNode[i + 1] = tempNode;
                        sorted = false;
                    }
                }

                sorted = true;
            }
        }

        /// <summary>
        /// Splits at index.
        /// </summary>
        /// <param name="nodeSplit">the node to be split</param>
        /// <param name="nodeAdd">node to be added</param>
        /// <param name="newValue">the new value to be added</param>
        public void splitIndex(Index nodeSplit, Node nodeAdd, int newValue)
        {
            List<Node> listNode = new List<Node>();
            List<int> listInt = new List<int>();

            Index index = new Index(nodeSize);

            listInt.Add(newValue);
            listNode.Add(nodeAdd);

            for (int i = 0; i < nodeSize; i++)
            {
                listInt.Add(nodeSplit.value[i]);
                listNode.Add(nodeSplit.Indexes[i]);
            }

            Sort(listInt, listNode);
            nodeSplit.value.Clear();
            nodeSplit.Indexes.Clear();

            int tempNodeSize = nodeSize + 1;

            for (int i = 0; i < tempNodeSize / 2; i++)
            {
                nodeSplit.value.Add(listInt[i]);
                nodeSplit.Indexes.Add(listNode[i]);
            }
            for (int i = tempNodeSize / 2; i < tempNodeSize; i++)
            {
                index.value.Add(listInt[i]);
                index.Indexes.Add(listNode[i]);
            }

            if (root == nodeSplit)
            {
                root = new Index(nodeSize);
                ((Index)root).Insert(nodeSplit.value[0], nodeSplit);
                ((Index)root).Insert(index.value[0], index);
            }
            else
            {
                stack.Pop();
                Index tempIndex = (Index)stack.Peek();
                INSERT insert = tempIndex.Insert(index.value[0], index);
                if (insert == INSERT.NEEDSPLIT)
                {
                    splitIndex(tempIndex, index, index.value[0]);
                }
            }
        }

        /// <summary>
        /// split at leaf
        /// </summary>
        /// <param name="inLeaf">the leaf passed in</param>
        /// <param name="inNum">the number passed in</param>
        public void splitLeaf(Leaf inLeaf, int inNum)
        {
            Leaf tempLeaf = new Leaf(nodeSize);
            List<int> tempList = new List<int>(inLeaf.value);
            tempList.Add(inNum);
            tempList.Sort();
            inLeaf.value.Clear();

            int temp = nodeSize + 1;

            for (int i = 0; i < temp / 2; i++)
            {
                inLeaf.value.Add(tempList[i]);
            }
            for (int i = temp / 2; i < nodeSize + 1; i++)
            {
                tempLeaf.value.Add(tempList[i]);
            }

            stack.Pop();
            Index tempIndex = (Index)stack.Peek();
            INSERT insert = tempIndex.Insert(tempLeaf.value[0], tempLeaf);

            if (insert == INSERT.NEEDSPLIT)
            {
                splitIndex(tempIndex, tempLeaf, tempLeaf.value[0]);
            }
        }

        /// <summary>
        /// returns a string that contains stats about the BTree.
        /// </summary>
        /// <returns>output string</returns>
        public string Stats()
        {
            string output = "The number of Index nodes is : " + indexCount + "\nThe number of leaf nodes is: " + leafCount + " with an average of "
                + (leafCount / nodeSize) + "% full." + "\nThe depth of the tree is: " + findDepth() + "\nThe total number of values in the tree is: " + count;

            return output;
        }

        #endregion Methods
    }
}