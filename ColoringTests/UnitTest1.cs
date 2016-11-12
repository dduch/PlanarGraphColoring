using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using PlanarGraphColoring;


namespace ColoringTests
{
    [TestClass]
    public class UnitTest1
    {
        private void CheckColoring(int[][] adjacencyLists)
        {
            var g = new Graph(adjacencyLists);
            var coloring = g.ColorFive();
            var usedColors = new int[6];

            for (var v = 0; v < adjacencyLists.Length; ++v)
            {
                var vColor = coloring[v];

                // Check if vertex color is in range 1..5
                if (vColor > 5 || vColor < 1)
                    throw new Exception("Vertex <" + v + "> colored using not allowed color: " + coloring[v]);

                usedColors[vColor] = 1;

                var neighborsOfV = adjacencyLists[v];
                foreach(var u in neighborsOfV)
                {
                    // Check if each 2 adjacent vertices have different colors
                    if (coloring[v] == coloring[u])
                        throw new Exception("Adjacent vertices <" + v + "> and <" + u + "> colored using the same color: " + coloring[v]);
                }
            }

            Console.WriteLine("Used colors count: " + usedColors.Sum());
        }

        // Test with simple graph of maximum degree 4
        [TestMethod]
        public void SimpleGraphMax4DegreeTest()
        {
            var data = new int[][]
            {
                new int[] {1,2,3,4},
                new int[] {0,4,5,6},
                new int[] {0,6,4,3},
                new int[] {0,2,4},
                new int[] {0,3,2,1},
                new int[] {1,6},
                new int[] {1,5,2},
            };
            CheckColoring(data);
        }

        // Test with simple graph of maximum degree 6
        [TestMethod]
        public void SimpleGraphMax6DegreeTest()
        {
            var data = new int[][]
            {
                new int[] {1,2,3,4,5,6},
                new int[] {0,6,3,2},
                new int[] {1,3,0},
                new int[] {2,1,6,4,0},
                new int[] {3,6,5,0},
                new int[] {4,6,0},
                new int[] {5,4,3,1,0}
            };
            CheckColoring(data);
        }

        // Icosahedron is a 5-regular planar graph
        [TestMethod]
        public void IcosahedronTest()
        {
            var data = new int[][]
            {
                new int[] {1,4,3,8,2},
                new int[] {2,6,5,4,0},
                new int[] {0,8,7,6,1},
                new int[] {0,4,10,9,8},
                new int[] {0,1,5,10,3},
                new int[] {4,1,9,11,10},
                new int[] {1,2,7,11,5},
                new int[] {6,2,8,9,11},
                new int[] {0,3,9,7,2},
                new int[] {3,10,11,7,8},
                new int[] {3,4,5,11,9},
                new int[] {5,6,7,9,10}
            };
            CheckColoring(data);
        }

    }
}
