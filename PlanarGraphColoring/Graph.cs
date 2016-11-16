using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanarGraphColoring
{
    public class Graph
    {
        public Vertex[] _vs;
        public Stack<Vertex> _s4; // Contains all remaining vertices with either degree at most 4
        public Stack<Vertex> _s5; // Contains all remaining vertices that have degree 5 and at least one adjacent vertex with degree at most 6
        public Stack<RemovedVertex> _sd; // Contains all vertices deleted from the graph so far, in the order that they were deleted

        public Graph(IEnumerable<IEnumerable<int>> ajdacencyLists)
        {
            var vCount = ajdacencyLists.Count();
            _vs = new Vertex[vCount];
            int i;

            for (i = 0; i < vCount; ++i)
                _vs[i] = new Vertex(i);

            i = 0;
            foreach(var adjList in ajdacencyLists)
            {
                _vs[i++].Neighbors = adjList.Select(vid => _vs[vid]);
            }
        }

        public int[] ColorFive()
        {
            _s4 = new Stack<Vertex>();
            _s5 = new Stack<Vertex>();
            _sd = new Stack<RemovedVertex>();

            // Iterate over the vertices of the graph, pushing any vertex matching the conditions for S4 or S5 onto the appropriate stack
            PushOnStacksIfConditionsMet(_vs);

            while(true)
            {
                // Next, as long as S4 is non-empty, we pop v from S4 and delete v from the graph, pushing it onto Sd, along with a list of its neighbors at this point in time. 
                // We check each former neighbor of v, pushing it onto S4 or S5 if it now meets the necessary conditions.
                while (_s4.Count > 0)
                {
                    var v = _s4.Pop();
                    _sd.Push(v.RemoveFromGraph());
                    PushOnStacksIfConditionsMet(v.Neighbors);
                }

                if (_sd.Count == _vs.Length)
                    break; // All vertices were removed

                Vertex v5;
                do
                {
                    v5 = _s5.Pop();
                } while (v5.IsRemoved);

                var neighbors = v5.Neighbors.Take(4).ToList();
                Vertex u, w;
                if (neighbors[0].IsAdjacentTo(neighbors[2]))
                {
                    u = neighbors[1];
                    w = neighbors[3];
                }
                else
                {
                    u = neighbors[0];
                    w = neighbors[2];
                }

                var removedVertexes = u.MergeWithAndRemove(w, v5);
                _sd.Push(removedVertexes.Item1);
                _sd.Push(removedVertexes.Item2);

                PushOnStacksIfConditionsMet(v5.Neighbors.Where(x => x.Id != w.Id).Union(u.Neighbors));
            }

            return ColorRemoved();
        }

        private void PushOnStacksIfConditionsMet(IEnumerable<Vertex> vs)
        {
            foreach (var v in vs)
            {
                if (v.Degree <= 4 && !v.ToBeRemoved)
                {
                    v.ToBeRemoved = true;
                    _s4.Push(v);
                }
                else if (v.Degree == 5)
                    _s5.Push(v);
            }
        }

        private int[] ColorRemoved()
        {
            var coloring = new int[_vs.Length];

            while(_sd.Count > 0)
            {
                var v = _sd.Pop();

                if (v.IsMerged)
                {
                    coloring[v.Id] = coloring[v.MergedWith];
                }
                else
                {
                    var colorsUsage = new bool[6]; // 1..5 for colors, 0 for not colored vertices
                    foreach(var n in v.Neighbors)
                    {
                        // Mark color of the neighbor as already used
                        colorsUsage[coloring[n]] = true;
                    }

                    // Find not used color
                    int i = 0;
                    while (++i < 6 && colorsUsage[i]) ;
                    if (i == 6)
                        throw new Exception("Graph cannot be colored with max 5 colors");
                    else
                        coloring[v.Id] = i;
                }
            }

            return coloring;
        }
    }
}
