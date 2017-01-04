using PlanarGraphColoring;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GraphTestDataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            RunColoring();
        }

        public static void RunColoring()
        {
            DirectoryInfo d = new DirectoryInfo("ParsedTestData");
            FileInfo[] Files = d.GetFiles();
            foreach (FileInfo file in Files)
            {
                int[][] graph = LoadGraph(file.FullName);
                var g = new Graph(graph);
                var coloring = g.ColorFive();

                StringBuilder sb = new StringBuilder();

                for(int i = 0; i < coloring.Length; ++i)
                {
                    sb.AppendLine((i + 1).ToString() + ": " + coloring[i]);
                }
                
                System.IO.File.WriteAllText(@"Colored\" + file.Name, sb.ToString());
            }
        }

        private static int[][] LoadGraph(string fileName)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            string line;
            int vertexes = File.ReadLines(fileName).Count();
            int[][] graph = new int[vertexes][];
            int counter = 0;

            while ((line = file.ReadLine()) != null)
            {
                if (line[line.Length - 1] == ' ')
                {
                    line = line.Substring(0, line.Length - 1);
                }
                string[] neighbours = line.Split(' ');
                graph[counter] = new int[neighbours.Length];

                for (int i = 0; i < neighbours.Length; ++i)
                {
                    graph[counter][i] = Convert.ToInt32(neighbours[i]) -1;
                }
                ++counter;
            }
            file.Close();
            return graph;
        }
    }
}




