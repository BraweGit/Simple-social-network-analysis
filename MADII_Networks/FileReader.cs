using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace MADII_Networks
{
    public class FileReader
    {
        private string path;
        private int[] rawData;
        private int[] parents;
        private int[] children;
        public FileReader(string p)
        {
            path = p;
            ReadFile();
        }

        public Graph GetGraph()
        {
            var g =  new Graph();
            var data = rawData.Distinct().OrderBy(x => x).ToArray();

            g.CreateRoot(data[0].ToString());
            for (int i = 1; i < data.Length; i++)
            {
                g.CreateNode(data[i].ToString());
            }

            foreach (var n in g.AllNodes)
            {
                for (int i = 0; i < parents.Length; i++)
                {
                    if (parents[i].ToString() == n.Name)
                        n.AddArc(g.AllNodes.FirstOrDefault(x => x.Name == children[i].ToString()), 1);
                }
            }
            return g;

        }

        private void ReadFile()
        {
            int counterLines = 0;
            int counterArcs = 0;
            string line;
            var file = new System.IO.StreamReader(path);
            var linesCount = File.ReadLines(path).Count();

            rawData = new int[linesCount * 2];
            parents = new int[linesCount];
            children = new int[linesCount];

            while ((line = file.ReadLine()) != null)
            {
                var split = line.Split(';');
                for (int i = 0; i < split.Length; i++)
                {
                    rawData[counterLines] = Int32.Parse(split[i]);
                    counterLines++;
                }
                parents[counterArcs] = Int32.Parse(split[0]);
                children[counterArcs] = Int32.Parse(split[1]);
                counterArcs++;
            }
        }

    }
}
