using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADII_Networks
{
    public class Node
    {
        public string Name;
        public double CC;
        public List<Arc> Arcs = new List<Arc>();
        public int Degree { get; set; }

        public Node(string name)
        {
            Name = name;
            Degree = 0;
        }

        /// <summary>
        /// Create a new arc, connecting this Node to the Node passed in the parameter
        /// Also, it creates the inversed node in the passed node
        /// </summary>
        public Node AddArc(Node child, int w)
        {
            Degree++;
            Arcs.Add(new Arc
            {
                Parent = this,
                Child = child,
                Weigth = w
            });

            if (!child.Arcs.Exists(a => a.Parent == child && a.Child == this))
            {
                child.AddArc(this, w);
            }

            return this;
        }

        /// <summary>
        /// Create a new arc, connecting this Node to the Node passed in the parameter
        /// Also, it creates the inversed node in the passed node
        /// </summary>
        public Node AddSingleArc(Node child, int w)
        {
            if (!child.Arcs.Exists(a => a.Child == this && a.Parent == child))
            {
                if (!Arcs.Exists(a => a.Parent == this && a.Child == child))
                {
                    Degree++;
                    Arcs.Add(new Arc
                    {
                        Parent = this,
                        Child = child,
                        Weigth = w
                    });
                }

            }

            return this;
        }

        public List<Node> GetNeighbors()
        {
            return (from a in Arcs select a.Child).ToList();
        }

        public void GetCC()
        {
            double kv = Arcs.Count;
            double nv = 0d;

            for (int i = 0; i < Arcs.Count; i++)
            {
                for (int j = i + 1; j < Arcs.Count; j++)
                {
                    foreach (var a in Arcs[j].Child.Arcs)
                    {
                        if (Arcs[i].Child.Name == a.Child.Name)
                            nv++;
                    }

                }
            }
            CC = (2 * nv) / (kv * (kv - 1));
        }


    }
}
