using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using PlanarGraphColoring;

namespace ColoringTests
{
    static class TestUtils
    {
        public static void CheckColoring(int[][] adjacencyLists)
        {
            var g = new Graph(adjacencyLists);
            var coloring = g.ColorFive();
            //SaveColouredGraph(coloring, file);

            for (var v = 0; v < adjacencyLists.Length; ++v)
            {
                var vColor = coloring[v];

                // Check if vertex color is in range 1..5
                if (vColor > 5 || vColor < 1)
                    throw new ColoringException("Vertex <" + v + "> colored using not allowed color: " + vColor, adjacencyLists, coloring);

                var neighborsOfV = adjacencyLists[v];
                foreach (var u in neighborsOfV)
                {
                    // Check if each 2 adjacent vertices have different colors
                    if (coloring[v] == coloring[u])
                        throw new ColoringException("Adjacent vertices <" + v + "> and <" + u + "> colored using the same color: " + coloring[v], adjacencyLists, coloring);
                }
            }
        }

        // Generates file (if not exists) containing planar code encoded graphs with specified number of vertexes and properties
        // Returns name of file with requested graphs
        public static string GenerateGraphsUsingPlantri(int verticesCount, string graphProps)
        {
            var props = graphProps != null ? ("-" + graphProps) : "";
            var outfile = "g" + verticesCount + props + ".pc"; // .pc for 'planar code'
            if (!File.Exists(outfile))
            {
                var arguments = verticesCount + " " + props + " " + outfile;
                var proc = Process.Start("plantri.exe", arguments);
                proc.WaitForExit();
            }
            return outfile;
        }

        public static LinkedList<int[][]> LoadGraphs(string filename)
        {
            var binData = File.ReadAllBytes(filename);
            int verticesCount = (binData.Length > 15) ? binData[15] : -1;
            var graphs = new LinkedList<int[][]>();

            int i = 16 - 2;
            while(++i < binData.Length)
            {
                var graph = new int[verticesCount][];
                for (int j = 0; j < verticesCount; ++j)
                {
                    var neighbors = new List<int>();
                    while (binData[++i] != 0)
                        neighbors.Add(binData[i] - 1);
                    graph[j] = neighbors.ToArray();
                }
                graphs.AddLast(graph);
            }

            return graphs;
        }

        public static string SaveColouredGraph(int[][] graph, int[] coloring, string filename, string message = "")
        {
            var outfile = "coloured" + filename + ".txt";
            StreamWriter file = new StreamWriter(outfile);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < coloring.Length; ++i)
            {
                sb.AppendLine("vertex: " + i + " color: " + coloring[i] + " neighbors: " + string.Join(" ", graph[i]));
            }
            sb.AppendLine(message);
            file.Write(sb.ToString());
            file.Close();
            return outfile;
        }

        //private static int[][] LoadGraph(string fileName)
        //{
        //    System.IO.StreamReader file = new System.IO.StreamReader(fileName);
        //    string line;
        //    int vertexes = File.ReadLines(fileName).Count();
        //    int[][] graph = new int[vertexes][];
        //    int counter = 0;

        //    while ((line = file.ReadLine()) != null)
        //    {
        //        if (line[line.Length - 1] == ' ')
        //        {
        //            line = line.Substring(0, line.Length - 1);
        //        }
        //        string[] neighbours = line.Split(' ');
        //        graph[counter] = new int[neighbours.Length];

        //        for (int i = 0; i < neighbours.Length; ++i)
        //        {
        //            graph[counter][i] = Convert.ToInt32(neighbours[i]);
        //        }
        //        ++counter;
        //    }
        //    file.Close();
        //    return graph;
        //}
    }
}
