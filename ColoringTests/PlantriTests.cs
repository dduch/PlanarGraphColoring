using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace ColoringTests
{
    [TestClass]
    public class PlantriTests
    {
        private void TestGraphClass(int verticesCount, string graphProps)
        {
            var graphFile = TestUtils.GenerateGraphsUsingPlantri(verticesCount, graphProps);
            var graphLoader = new PlantriGraphLoader(graphFile);
            // For collecting coloring time for each graph grouped by edges count
            var coloringMetadata = new int[Math.Max(verticesCount * 3 - 5, 4)].Select(dontCare => new List<long>()).ToArray();  // Swiftly initialize array

            int graphsCount = 0;
            try
            {
                var adjLists = graphLoader.LoadGraph();
                while (adjLists != null)
                {
                    var meta = TestUtils.CheckColoring(adjLists);   
                    ++graphsCount;
                    coloringMetadata[meta.Edges].Add(meta.Microseconds);
                    adjLists = graphLoader.LoadGraph();
                }
            }
            catch (ColoringException ex)
            {
                var logfile = TestUtils.SaveColouredGraph(ex.Graph, ex.Coloring, verticesCount + graphProps, ex.Message);
                throw;
            }

            Console.WriteLine("Successfully colored " + graphsCount + " graphs with " + verticesCount + " vertices of class " + graphProps);

            var coloringMetadataLines = coloringMetadata.Select((timeList, i) =>
            {
                var count = timeList.Count;
                var avg = (count != 0) ? (timeList.Sum() * 1000 / count) : 0;
                var max = (count != 0) ? timeList.Max() : 0;
                var min = (count != 0) ? timeList.Min() : 0;
                return i + "\t"  + count + "\t" + avg + "\t" + min + "\t" + max;
            }).ToList();

            coloringMetadataLines[0] = "Edges\tGraphs\tAvg time [us]\tMin time [us]\tMax time [us]";


            File.WriteAllLines("v" + verticesCount + "summary.csv", coloringMetadataLines);
        }


        [TestMethod]
        public void AllGraphsFrom2To8Vertices()
        {
            int verticesCount;
            string graphProps = "pm1c1";
            for(verticesCount = 2; verticesCount <= 8; ++verticesCount)
                TestGraphClass(verticesCount, graphProps);
        }

        [TestMethod]
        public void AllGraphsWith9Vertices()
        {
            int verticesCount = 9;
            string graphProps = "pm1c1";
            TestGraphClass(verticesCount, graphProps);
        }

        [TestMethod]
        [Ignore]
        public void AllGraphsWith10Vertices()
        {
            int verticesCount = 10;
            string graphProps = "pm1c1";
            TestGraphClass(verticesCount, graphProps);
        }

        [TestMethod]
        public void AllGraphsFrom12To23VerticesAndMinDegree5()
        {
            int verticesCount;
            string graphProps = "pm5";
            for (verticesCount = 12; verticesCount <= 23; ++verticesCount)
                TestGraphClass(verticesCount, graphProps);
        }

        [TestMethod]
        public void AllGraphsWith24VerticesAndMinDegree5()
        {
            int verticesCount = 24;
            string graphProps = "pm5";
            TestGraphClass(verticesCount, graphProps);
        }

        [TestMethod]
        [Ignore]
        public void AllGraphsWith25VerticesAndMinDegree5()
        {
            int verticesCount = 25;
            string graphProps = "pm5";
            TestGraphClass(verticesCount, graphProps);
        }
    }
}
