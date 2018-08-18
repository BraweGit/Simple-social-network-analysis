using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADII_Networks
{
    public class Graph
    {
        public Node Root;
        public double AverageCC;
        private int inf = 9999;
        public List<Node> AllNodes = new List<Node>();
        public int ArcsCount
        {
            get
            {
                var sum = 0;
                foreach (var n in AllNodes)
                {
                    sum += n.Arcs.Count;
                }
                return sum;
            }
        }

        public Node CreateRoot(string name)
        {
            Root = CreateNode(name);
            return Root;
        }

        public Node CreateNode(string name)
        {
            var n = new Node(name);
            AllNodes.Add(n);
            return n;
        }

        public int[][] CreateAdjMatrix()
        {
            int[][] adj = new int[AllNodes.Count][];
            for (int i = 0; i < adj.Length; i++)
            {
                adj[i] = new int[AllNodes.Count];
            }

            for (int i = 0; i < AllNodes.Count; i++)
            {
                var n1 = AllNodes[i];

                for (int j = 0; j < AllNodes.Count; j++)
                {
                    var n2 = AllNodes[j];

                    var arc = n1.Arcs.FirstOrDefault(a => a.Child == n2);

                    if (arc != null)
                    {
                        adj[i][j] = 1; // arc.Weigth if we want cost of path
                    }
                    else
                    {
                        adj[i][j] = 0;
                    }
                }
            }
            return adj;
        }

        /*
         | 0, 1, 0, 1, 0 |
         | 0, 0, 1, 0, 0 |
         | 1, 1, 0, 1, 0 | 
         | 1, 0, 0, 0, 1 |
         | 1, 0, 0, 0, 0 |
         
         */

        public List<List<int>> KCore(int[][] adj, int k, List<List<int>> result = null, List<int> kCores = null)
        {
            if (k == 0) return result;
            else
            {
                if(kCores == null)
                    result = new List<List<int>>();
                //First get all nodes with k degree
                var kDegree = new List<int>();

                for (int i = 0; i < adj.Length; i++)
                {
                    var degree = 0;
                    for (int j = 0; j < adj.Length; j++)
                    {
                        if (i != j)
                        {
                            if (adj[i][j] > 0) degree++;
                        }

                        if (degree == k)
                        {
                            kDegree.Add(i);
                            break;
                        }
                    }
                }

                //Keep removing nodes which dont have k neighbors in k-core subset
                var changed = true;
                while (changed)
                {
                    changed = false;
                    for (int i = 0; i < kDegree.Count; i++)
                    {
                        var allNeighbors = GetNeighborsAdj(adj, kDegree[i]);
                        var kCoreNeighbors = kDegree.Intersect(allNeighbors).ToList();
                        if (kCoreNeighbors.Count < k)
                        {
                            kDegree.RemoveAt(i);
                            i--;
                            changed = true;
                        }
                    }
                }

                if (kCores != null)
                    kDegree = kDegree.Except(kCores).ToList();

                result.Add(kDegree);

                return KCore(adj, k - 1, result, kCores == null ? kDegree : kDegree.Union(kCores).ToList());
            }

        }

        private List<int> GetNeighborsAdj(int[][] adj, int node)
        {
            var result = new List<int>();

            for (int j = 0; j < adj.Length; j++)
            {
                if (adj[node][j] > 0 && node != j) result.Add(j);
            }
            return result;
        }

        public List<List<int>> NCliques(int[][] adj, int n)
        {
            var result = new List<List<int>>();

            for (int i = 0; i < adj.Length; i++)
            {
                var tmp = new List<List<int>>();
                for (int j = 0; j < adj.Length; j++)
                {
                    if (adj[i][j] > 0)
                    {
                        for (int k = 0; k < adj.Length; k++)
                        {
                            var cliq = new List<int>();

                            if (adj[j][k] > 0)
                            {
                                if (!cliq.Contains(i))
                                    cliq.Add(i);
                                if (!cliq.Contains(j))
                                    cliq.Add(j);
                                if (!cliq.Contains(k))
                                    cliq.Add(k);
                            }
                            if (cliq.Count > 2)
                            {
                                if (tmp.Count > 0)
                                {
                                    for (int l = 0; l < tmp.Count; l++)
                                    {
                                        if (tmp[l].Except(cliq).Any())
                                        {
                                            tmp.Add(cliq);
                                            break;
                                        }
                                    }
                                    
                                }
                                else tmp.Add(cliq);

                            }
                                
                        }
                    }

                    
                }
                result.AddRange(tmp);
            }

            var res = new List<List<int>>();

            for (int i = 0; i < result.Count; i++)
            {
                var tmpList = new List<List<int>>();
                for (int j = 0; j < result.Count; j++)
                {
                    var tmp = new List<int>();
                    if (i != j)
                    {
                        if (adj[result[i][2]][result[j][2]] > 0)
                        {
                            foreach (var item in result[i])
                            {
                                if(!tmp.Contains(item)) tmp.Add(item);
                            }

                            foreach (var item in result[j])
                            {
                                if (!tmp.Contains(item)) tmp.Add(item);
                            }
                        }
                    }
                    if (tmp.Count > 0)
                    {
                        if (tmpList.Count > 0)
                        {
                            for (int l = 0; l < tmpList.Count; l++)
                            {
                                if (tmpList[l].Except(tmp).Any())
                                {
                                    tmpList.Add(tmp);
                                    break;
                                }
                            }

                        }
                        else tmpList.Add(tmp);
                    }
         }
                res.AddRange(tmpList);

            }

            res = RemoveDuplicates(res);
            return res;
        }

        private List<List<int>> RemoveDuplicates(List<List<int>> duplicates)
        {
            //Remove subsets of lists in collection
            var noDuplicates = duplicates;
            //Remove last duplicates
            var change = true;
            while (change)
            {
                change = false;
                for (int i = 0; i < duplicates.Count; i++)
                {
                    for (int j = 0; j < duplicates.Count; j++)
                    {
                        if (i != j)
                        {
                            if (!duplicates[i].Except(duplicates[j]).Any())
                            {
                                noDuplicates.Remove(duplicates[j]);
                                change = true;
                            }
                        }
                        if (change) break;
                    }
                    if (change) break;
                }
            }


            return noDuplicates;
        }

        private void AddChildren(LinkedList<Tree<int>> children, int[][] adj, int n)
        {
            if (n == 0) return;
            else
            {
                var newChildren = new LinkedList<Tree<int>>();
                foreach (var child in children)
                {
                    var neighbors = GetNeighborsAdj(adj, child.GetData<int>());
                    foreach (var neigh in neighbors)
                    {
                        child.AddChild(neigh);
                    }

                    var added = child.GetChildren();
                    foreach (var a in added)
                    {
                        newChildren.AddFirst(a);
                    }
                }


                AddChildren(newChildren, adj, n - 1);
            }


        }

        private void AddNeighbors(int[][] adj, List<List<int>> l)
        {
            var tmp = l;
            //Add neighbors of nodes in list
            //for (int i = 0; i < l.Count; i++)
            //{
            //    for (int j = 0; j < l.Count; j++)
            //    {
            //        if (i != j)
            //        {
            //            if (l[i].Select(x => x).Intersect(l[j]).Any())
            //            {
            //                l[i] = l[i].Union(l[j]).ToList();
            //                l.Remove(l[j]);
            //            }
            //        }
            //    }
            //}

            //l = RemoveDuplicates(l);
            //Every list
            for (int i = 0; i < l.Count; i++)
            {
                //Every neighbor
                for (int j = 0; j < l[i].Count; j++)
                {
                    if (i != j)
                    {
                        var allNeighbors = GetNeighborsAdj(adj, l[i][j]);
                        l[i] = l[i].Union(allNeighbors).ToList();
                    }

                }
            }
        }

        public float[][] CreateNormalizedAdjMatrix(int[][] adj)
        {
            //We will need array of doubles instead of integers
            var norm = new float[AllNodes.Count][];
            for (int i = 0; i < adj.Length; i++)
            {
                norm[i] = new float[AllNodes.Count];
            }

            for (int i = 0; i < AllNodes.Count; i++)
            {
                var n1 = AllNodes[i];

                for (int j = 0; j < AllNodes.Count; j++)
                {
                    var n2 = AllNodes[j];

                    var arc = n1.Arcs.FirstOrDefault(a => a.Child == n2);

                    if (arc != null)
                    {
                        norm[i][j] = 1.0f / n1.Degree; // arc.Weigth if we want cost of path

                    }
                    else
                    {
                        norm[i][j] = 0.0f / n1.Degree;
                    }
                }

            }
            return norm;
        }


        public int[][] CreateDegreeMatrix(int[][] adj)
        {
            //Create degree matrix
            var deg = new int[AllNodes.Count][];
            for (int i = 0; i < adj.Length; i++)
            {
                deg[i] = new int[AllNodes.Count];
            }

            for (int i = 0; i < AllNodes.Count; i++)
            {
                deg[i][i] = adj[i].Sum();
            }
            return deg;
        }

        public int[][] CreateLaplacianMatrix(int[][] adj, int[][] degree)
        {
            //Create degree matrix
            var lap = new int[AllNodes.Count][];
            for (int i = 0; i < adj.Length; i++)
            {
                lap[i] = new int[AllNodes.Count];
            }

            for (int i = 0; i < AllNodes.Count; i++)
            {
                for (int j = 0; j < AllNodes.Count; j++)
                {
                    lap[i][j] = degree[i][j] - adj[i][j];
                }
            }
            return lap;
        }



        public int pathLength(int[][] matrix, int from, int to)
        {
            if (matrix[from - 1][to - 1] != inf)
            {
                return matrix[from - 1][to - 1];
            }
            else
            {
                return matrix[from - 1][to - 1];
            }
        }

        public void CountCC()
        {
            foreach (var n in AllNodes)
            {
                n.GetCC();
            }
        }

        public void CountAverageCC()
        {
            var sumCC = 0d;
            foreach (var n in AllNodes)
            {
                sumCC += n.CC;
            }

            AverageCC = sumCC / AllNodes.Count;
        }

        public float AverageShortestPath(int[][] dist)
        {
            float avgShortestPath = 0f;
            int counterShortestPath = 0;
            int max = 0;
            int min = inf;

            for (var i = 0; i < AllNodes.Count; i++)
            {
                for (var j = 0; j < AllNodes.Count; j++)
                {
                    int temp = (int)pathLength(dist, i + 1, j + 1);
                    if (temp != inf && temp != 0)
                    {
                        avgShortestPath += temp;
                        counterShortestPath++;

                        if (temp > max) max = temp;
                        if (temp < min) min = temp;
                    }
                }
            }

            avgShortestPath = (float)avgShortestPath / (float)counterShortestPath;
            Console.WriteLine("Average shortest path is: {0}", avgShortestPath);
            Console.WriteLine("Average graph is: {0}", max);
            return avgShortestPath;
        }

        public void ClosenessCentrality(int[][] dist)
        {
            var avg = new List<float>();
            for (var i = 0; i < AllNodes.Count; i++)
            {
                var suma = 0f;

                for (var j = 0; j < AllNodes.Count; j++)
                {
                    int? temp = pathLength(dist, i + 1, j + 1);
                    suma += (float)temp;
                }

                suma = AllNodes.Count / suma;
                avg.Add(suma);

                Console.WriteLine("Centrality point for {0} is: {1}", i + 1, suma);

            }
            Console.WriteLine("Average closeness centrality: {0}", avg.Average());
        }

        public void PrintFloyd(int?[,] dist)
        {
            for (int i = 0; i < AllNodes.Count; ++i)
            {
                for (int j = 0; j < AllNodes.Count; ++j)
                {
                    if (dist[i, j] == inf)
                        Console.Write("? ");
                    else
                        Console.Write(dist[i, j] + " ");
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }


        public int?[,] FloydWarshall(int?[,] m)
        {
            int?[,] paths = new int?[AllNodes.Count, AllNodes.Count];

            for (var i = 0; i < AllNodes.Count; i++)
            {
                for (var j = 0; j < AllNodes.Count; j++)
                {
                    if (m[i, j] == null && i != j)
                    {
                        paths[i, j] = inf;
                    }
                    else if (i == j)
                    {
                        paths[i, j] = 0;
                    }
                    else
                    {
                        paths[i, j] = m[i, j];
                    }

                }
            }

            for (var k = 0; k < AllNodes.Count; k++)
            {
                // Pick all vertices as source one by one
                for (var i = 0; i < AllNodes.Count; i++)
                {
                    // Pick all vertices as destination for the
                    // above picked source
                    for (var j = 0; j < AllNodes.Count; j++)
                    {
                        // If vertex k is on the shortest path from
                        // i to j, then update the value of dist[i][j]
                        if (paths[i, k] + paths[k, j] < paths[i, j])
                            paths[i, j] = paths[i, k] + paths[k, j];
                    }
                }
            }


            return paths;
        }

        public void BuildRandomGraph(int n, double p)
        {
            var r = new Random();
            // Generate nodes
            CreateRoot(0.ToString());
            for (int i = 1; i < n; i++)
            {
                CreateNode(i.ToString());
            }

            for (int i = 0; i < AllNodes.Count; i++)
            {
                for (int j = i + 1; j < AllNodes.Count; j++)
                {
                    var rnd = r.NextDouble();
                    if (rnd < p && i != j)
                    {
                        AllNodes[i].AddArc(AllNodes[j], 1);
                    }

                }
            }
        }


        // p = stupen 
        // m = 3,  int - number of edges to attach from a new node to existing nodes; the number of edges to be added at each step
        // n_0 => m,  int - number of nodes
        // v kazdem kroku 1 novy vrchol ktery se spoji prave s m vrcholy, plati preferencni pripojovani p = stupen vrcholu

        private List<Node> Neighbors(Graph g)
        {
            var result = new List<Node>();
            foreach (var n in g.AllNodes)
            {
                for (int i = 0; i < n.Arcs.Count; i++)
                {
                    result.Add(n);
                }
            }
            return result;
        }


        private Node PickNode()
        {
            var neighbors = Neighbors(this);
            var r = new Random();

            return neighbors[r.Next(neighbors.Count)];
        }

        // n = pocet hran na pridani
        // m = pocatecni pocet vrcholu
        // e = new node edges
        public void BuildBAGraph(int m, int n, int total)
        {

            // Create initial number of nodes        
            for (int i = 0; i < m; i++)
            {
                CreateNode((AllNodes.Count + 1).ToString());
            }

            // Create complete graph
            for (int i = 0; i < AllNodes.Count; i++)
            {
                for (int j = i + 1; j < AllNodes.Count; j++)
                {
                    AllNodes[i].AddArc(AllNodes[j], 1);
                }
            }

            Node newNode = null;
            for (int i = m; i < total; i++)
            {
                newNode = new Node((AllNodes.Count + 1).ToString());
                for (int j = 0; j < n; j++)
                {
                    var pickNode = PickNode();
                    newNode.AddArc(pickNode, 1);
                }

                AllNodes.Add(newNode);
            }



        }
    }
}