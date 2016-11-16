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
            DirectoryInfo d = new DirectoryInfo("GeneratedTestData");
            FileInfo[] Files = d.GetFiles();
            foreach (FileInfo file in Files)
            {
                ParseFile(file.FullName, file.Name);
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
                        sb.Append(binData[i] - 1);
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
    }
}




