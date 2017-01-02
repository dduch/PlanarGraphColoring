using PlanarGraphColoring;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphTestDataParser
{
    class Program
    {
        static void Main(string[] args)
        {
            RunColoring();
        }

        public void ParsePlantriOutput()
        {
            DirectoryInfo d = new DirectoryInfo("GeneratedTestData");
            FileInfo[] Files = d.GetFiles();
            foreach (FileInfo file in Files)
            {
                ParseFile(file.FullName, file.Name);
            }
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

        private static void ParseFile(string fullFileName, string fName)
        {
            var binData = File.ReadAllBytes(fullFileName);
            int graphNumber = 1;
            System.IO.StreamWriter file = new System.IO.StreamWriter(@"ParsedTestData\out" + fName + "-" + graphNumber + ".txt");

            string result = System.Text.Encoding.ASCII.GetString(binData);
            int maxlines = 1;
            for (int i = 0, j = 0; i < binData.Length && j < maxlines; i++)
            {
                if (i < 15)
                {
                    continue;
                }
                else if (i == 15)
                {
                    maxlines = binData[i];
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    while (binData[i] != 0)
                    {
                        sb.Append(binData[i]);
                        sb.Append(" ");
                        ++i;
                    }

                    file.WriteLine(sb.ToString());
                    ++j;
                }

                if (j == maxlines)
                {
                    j = 0;
                    file.Close();
                    ++graphNumber;

                    if (i != binData.Length - 1)
                    {
                        file = new System.IO.StreamWriter(@"ParsedTestData\out" + fName + "-" + graphNumber + ".txt");
                        ++i;
                    }
                }
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




