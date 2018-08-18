using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MADII_Networks
{
    class Tree<T>
    {
        private T data;
        private LinkedList<Tree<T>> children;

        public Tree(T data)
        {
            this.data = data;
            children = new LinkedList<Tree<T>>();
        }

        public T GetData<T>()
        {
            return (T)Convert.ChangeType(data, typeof(T));
        }

        public void AddChild(T data)
        {
            children.AddFirst(new Tree<T>(data));
        }

        public LinkedList<Tree<T>> GetChildren()
        {
            return children;
        }

        public Tree<T> GetChild(int i)
        {
            foreach (Tree<T> n in children)
                if (--i == -1)
                    return n;
            return null;
        }

        public List<List<int>> GetChildrenList(List<List<int>> childrenList)
        {
            if (children.Count == 0) return childrenList;
            else
            {
                foreach (var child in children)
                {
                    var tmp = new List<int>();
                    foreach (var ch in child.children)
                    {
                        tmp.Add(ch.GetData<int>());
                    }
                    childrenList.Add(tmp);
                    child.GetChildrenList(childrenList);
                }
            }
            return null;
        }
    }
}
