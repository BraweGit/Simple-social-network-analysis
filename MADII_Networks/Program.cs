using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Math.Decompositions;

namespace MADII_Networks
{
    class Program
    {
        static void Print<T>(List<List<T>> toPrint)
        {
            for (int i = 0; i < toPrint.Count; i++)
            {
                Console.Write("ID: {0}, Items:  ", i);

                for (int j = 0; j < toPrint[i].Count; j++)
                {
                    if (j != toPrint[i].Count - 1)
                        Console.Write(toPrint[i][j] + ", ");
                    else Console.Write(toPrint[i][j] + "\n");
                }
            }
        }

        static void Main(string[] args)
        {
            //File to read
            var path = "KarateClub.csv";

            //Read *.csv file
            var fileReader = new FileReader(path);

            //Get graph from *.csv file
            var graph = fileReader.GetGraph();

            //Adjacency matrix
            var adjMatrix = graph.CreateAdjMatrix();

            //Normalized Adjacency matrix
            var normalizedAdjMatrix = graph.CreateNormalizedAdjMatrix(adjMatrix);

            //Laplacian matrix
            //First create degree matrix
            var degreeMatrix = graph.CreateDegreeMatrix(adjMatrix);
            //Then get laplacian matrix
            var laplacianMatrix = graph.CreateLaplacianMatrix(adjMatrix, degreeMatrix);

            //Eigenvalues of normalized Adjacency matrix
            var eigenValues = new JaggedEigenvalueDecompositionF(normalizedAdjMatrix).RealEigenvalues;

            //N-cliques
            var n = 2;
            var nCliques = graph.NCliques(adjMatrix, n);
            //Console.WriteLine("{0}-Cliques", n);
            //Print(nCliques);

            //K-Core
            var k = 4;
            var kCore = graph.KCore(adjMatrix, k);
            Console.ReadKey();
        }
    }
}
