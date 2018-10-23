using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

namespace SMP.ViewModels
{
    public class TreeViewModel<T>
    {
        public List<TreeViewNode<T>> Nodes { get; set; }

        public static TreeViewModel<T> BuildTree(List<T> roots, Func<T, List<T>> childrenPropFunc)
        {
            var res = new TreeViewModel<T> { Nodes = new List<TreeViewNode<T>>() };
            foreach (var root in roots)
            {
                res.Nodes.Add(BuildNode(root, childrenPropFunc));
            }

            return res;
        }

        private static TreeViewNode<T> BuildNode(T root, Func<T, List<T>> childrenPropFunc)
        {
            var res = new TreeViewNode<T> { Element = root };
            var children = childrenPropFunc(root);
            if (children.Any())
            {
                res.NextLevelElements = new List<TreeViewNode<T>>();
                foreach (var child in children)
                {
                    res.NextLevelElements.Add(BuildNode(child, childrenPropFunc));
                }
            }

            return res;
        }
    }

    public class TreeViewNode<T>
    {
        public T Element { get; set; }
        public List<TreeViewNode<T>> NextLevelElements { get; set; }
    }
}