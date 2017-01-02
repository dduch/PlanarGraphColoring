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
    class ColoringMeta
    {
        public int Vertices;
        public int Edges;
        public long Microseconds;
    }

    static class TestUtils
    {
        public static ColoringMeta CheckColoring(int[][] adjacencyLists)
        {
            var g = new Graph(adjacencyLists);

            var watch = Stopwatch.StartNew();
            var coloring = g.ColorFive();
            watch.Stop();

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

            return new ColoringMeta() { Vertices = g.VerticesCount, Edges = g.EdgesCount, Microseconds = 1000000 * watch.ElapsedTicks / Stopwatch.Frequency  };
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
    }
}
