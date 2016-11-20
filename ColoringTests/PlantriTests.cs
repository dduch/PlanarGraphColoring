using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace ColoringTests
{
    [TestClass]
    public class PlantriTests
    {
        private void TestGraphClass(int verticesCount, string graphProps)
        {
            var graphFile = TestUtils.GenerateGraphsUsingPlantri(verticesCount, graphProps);
            var graphsList = TestUtils.LoadGraphs(graphFile);

            LinkedListNode<int[][]> node;
            try
            {
                for (node = graphsList.First; node != null; node = node.Next)
                {
                    TestUtils.CheckColoring(node.Value);
                }
            }
            catch (ColoringException ex)
            {
                var logfile = TestUtils.SaveColouredGraph(ex.Graph, ex.Coloring, verticesCount + graphProps, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            Console.WriteLine("Successfully colored " + graphsList.Count + " graphs with " + verticesCount + " vertices of class " + graphProps);
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
        //[Ignore] // Takes ~2 minutes to complete
        public void AllGraphsWith25VerticesAndMinDegree5()
        {
            int verticesCount = 25;
            string graphProps = "pm5";
            TestGraphClass(verticesCount, graphProps);
        }
    }
}
