using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using PlanarGraphColoring;
using System.IO;
using System.Text;

namespace ColoringTests
{
    [TestClass]
    public class UnitTest1
    {
        private void CheckColoring(int[][] adjacencyLists, FileInfo file)
        {
            var g = new Graph(adjacencyLists);
            var coloring = g.ColorFive();
            var usedColors = new int[6];
            SaveColouredGraph(coloring, file);

            for (var v = 0; v < adjacencyLists.Length; ++v)
            {
                var vColor = coloring[v];

                // Check if vertex color is in range 1..5
                if (vColor > 5 || vColor < 1)
                    throw new Exception("Vertex <" + v + "> colored using not allowed color: " + coloring[v]);

                usedColors[vColor] = 1;

                var neighborsOfV = adjacencyLists[v];
                foreach (var u in neighborsOfV)
                {
                    // Check if each 2 adjacent vertices have different colors
                    if (coloring[v] == coloring[u])
                        throw new Exception("Adjacent vertices <" + v + "> and <" + u + "> colored using the same color: " + coloring[v]);
                }
            }

            Console.WriteLine("Used colors count: " + usedColors.Sum());
        }

        //Test with simple graph of maximum degree 4
        //[TestMethod]
        //public void SimpleGraphMax4DegreeTest()
        //{
        //    var data = new int[][]
        //    {
        //        new int[] {1,2,3,4},
        //        new int[] {0,4,5,6},
        //        new int[] {0,6,4,3},
        //        new int[] {0,2,4},
        //        new int[] {0,3,2,1},
        //        new int[] {1,6},
        //        new int[] {1,5,2},
        //    };
        //    CheckColoring(data);
        //}

        //// Test with simple graph of maximum degree 6
        //[TestMethod]
        //public void SimpleGraphMax6DegreeTest()
        //{
        //    var data = new int[][]
        //    {
        //        new int[] {1,2,3,4,5,6},
        //        new int[] {0,6,3,2},
        //        new int[] {1,3,0},
        //        new int[] {2,1,6,4,0},
        //        new int[] {3,6,5,0},
        //        new int[] {4,6,0},
        //        new int[] {5,4,3,1,0}
        //    };
        //    CheckColoring(data);
        //}

        // Icosahedron is a 5-regular planar graph
        //[TestMethod]
        //public void IcosahedronTest()
        //{
        //    var data = new int[][]
        //    {
        //        new int[] {1,4,3,8,2},
        //        new int[] {2,6,5,4,0},
        //        new int[] {0,8,7,6,1},
        //        new int[] {0,4,10,9,8},
        //        new int[] {0,1,5,10,3},
        //        new int[] {4,1,9,11,10},
        //        new int[] {1,2,7,11,5},
        //        new int[] {6,2,8,9,11},
        //        new int[] {0,3,9,7,2},
        //        new int[] {3,10,11,7,8},
        //        new int[] {3,4,5,11,9},
        //        new int[] {5,6,7,9,10}
        //    };
        //    CheckColoring(data);
        //}

        [TestMethod]
        public void TestFile1()
        {
            DirectoryInfo d = new DirectoryInfo(@"C:\Users\Dawid Dominiak\Documents\visual studio 2015\Projects\PlanarGraphColoring\ColoringTests\TestData\");
            FileInfo[] Files = d.GetFiles();
            foreach (FileInfo file in Files)
            {
                TestColoring(file.FullName, file);
            }
        }

        private void TestColoring(string path, FileInfo file)
        {
            var graph = LoadGraph(path);
            CheckColoring(graph, file);
        }

        private int[][] LoadGraph(string fileName)
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
                    graph[counter][i] = Convert.ToInt32(neighbours[i]);
                }
                ++counter;
            }
            file.Close();
            return graph;
        }

        private void SaveColouredGraph(int[] coloring, FileInfo fileInfo)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("coloured" + fileInfo.Name);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < coloring.Length; ++i)
            {
                sb.AppendLine(coloring[i].ToString());
            }
            file.Write(sb.ToString());
            file.Close();
        }
    }
}
