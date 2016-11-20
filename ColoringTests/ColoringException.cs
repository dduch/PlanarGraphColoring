using System;

namespace ColoringTests
{
    class ColoringException : Exception
    {
        public int[][] Graph { get; }
        public int[] Coloring { get; }

        public ColoringException(string msg, int[][] graph, int[] coloring) : base(msg)
        {
            Graph = graph;
            Coloring = coloring;
        }
    }
}
