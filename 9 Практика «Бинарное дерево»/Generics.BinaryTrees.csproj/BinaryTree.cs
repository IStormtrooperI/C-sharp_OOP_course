using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
    where T : IComparable {
        public T Value { get; set; }

        public BinaryTree<T> Left { get; set; }

        public BinaryTree<T> Right { get; set; }

        public void Add(T element)
        {
            if (Value.ToString() == "0")
                Value = element;
            else if (element.CompareTo(Value) > 0)
            {
                if (Right == null)
                    Right = new BinaryTree<T> { Value = element };
                else
                    Right.Add(element);
            }
            else
            {
                if(Left == null)
                    Left = new BinaryTree<T> { Value = element };
                else
                    Left.Add(element);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if(Value.ToString() == "0")
                return Enumerable.Empty<T>().GetEnumerator();
            return GetElementsToEnumerator();
        }

        private IEnumerator<T> GetElementsToEnumerator()
        {
            if (Left != null)
            {
                if (Left.Value != null && Left.Left == null && Left.Right == null)
                    yield return Left.Value;
                else
                {
                    foreach (T element in Left)
                        yield return element;
                }
            }
            if (Value.ToString() != "0")
                yield return Value;
            if (Right != null)
            {
                if (Right.Value != null && Right.Left == null && Right.Right == null)
                    yield return Right.Value;
                else
                {
                    foreach (T element in Right)
                        yield return element;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] list)
        where T : IComparable {
            BinaryTree<T> tree = new BinaryTree<T>();
            foreach (T element in list)
                tree.Add(element);
            return tree;
        }
    }
}
