using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.TreeTraversal
{
    public static class Traversal
    {
        private static IEnumerable<T> TreeTraversal<TType, T>(
            TType tree,
            Func<TType, bool> isNotNull,
            Func<TType, IEnumerable<TType>> treeTraversal,
            Func<TType, IEnumerable<T>> currentValue
            )
        {
            if (tree == null)
                yield break;

            if (isNotNull(tree))
                foreach (var value in currentValue(tree))
                {
                    var isInt32 = treeTraversal.ToString().Contains("System.Int32");
                    if (isInt32)
                    {
                        var isNullCurrentLeftAndRight = 
                            treeTraversal(tree).ToList()[0] == null && 
                            treeTraversal(tree).ToList()[1] == null;
                        if (isNullCurrentLeftAndRight)
                            yield return value;
                    }
                    if(!isInt32)
                        yield return value;
                }

            foreach (var currentTree in treeTraversal(tree))
                foreach (var value in TreeTraversal(currentTree, isNotNull, treeTraversal, currentValue))
                    yield return value;
        }

        public static IEnumerable<Product> GetProducts(ProductCategory root)
        {
            return TreeTraversal(root, r => r != null, r => r.Categories, r => r.Products);
        }

        public static IEnumerable<Job> GetEndJobs(Job root)
        {
            return TreeTraversal(root, r => r.Subjobs.Count == 0, r => r.Subjobs, r => new List<Job> {r});
        }

        public static IEnumerable<T> GetBinaryTreeValues<T>(BinaryTree<T> root)
        {
            return TreeTraversal(root, 
                r => r != null, 
                r => new List<BinaryTree<T>> {r.Left, r.Right}, 
                r => new List<T> {r.Value});
        }
    }
}