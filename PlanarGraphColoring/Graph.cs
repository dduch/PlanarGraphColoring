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

        public int VerticesCount { get; private set; }
        public int EdgesCount { get; private set; }

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

            VerticesCount = _vs.Length;
            EdgesCount = _vs.Sum(v => v.Degree) / 2;
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
                Vertex v, u, w;
                // Next, as long as S4 is non-empty, we pop v from S4 and delete v from the graph, pushing it onto Sd, along with a list of its neighbors at this point in time. 
                // We check each former neighbor of v, pushing it onto S4 or S5 if it now meets the necessary conditions.
                while (_s4.Count > 0)
                {
                    v = _s4.Pop();
                    _sd.Push(v.RemoveFromGraph());
                    PushOnStacksIfConditionsMet(v.Neighbors);
                }

                if (_sd.Count == _vs.Length)
                    break; // All vertices were removed, graph is empty -> proceed to final step - coloring

                // When S4 is empty our graph has minimum degree 5
                // Wernicke's Theorem tells us that S5 is nonempty
                // Pop v off S5
                do
                {
                    v = _s5.Pop();
                } while (v.IsRemoved); // Omit vertices previously removed from graph

                var neighborsTmp = v.Neighbors.ToList();
                var d6neighborIdx = neighborsTmp.FindIndex(x => x.Degree <= 6);
                var neighbors = new Vertex[5];
                for(var i = 0; i < 5; ++i)
                {
                    neighbors[i] = neighborsTmp[(i + d6neighborIdx) % 5];
                }

                // Find u and w - nonadjacent neighbors of v
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

                // Merge u and w into a single vertex. 
                // To do this, we remove v from both circular adjacency lists, and then splice the two lists together into one list at the point where v was formerly found.
                // It's possible that this might create faces bounded by two edges at the two points where the lists are spliced together; 
                // we delete one edge from any such faces. After doing this, we push v and w onto Sd, along with a note that u is the vertex that w was merged with. 
                var removedVertexes = u.MergeWithAndRemove(w, v);
                _sd.Push(removedVertexes.Item1);
                _sd.Push(removedVertexes.Item2);

                // Any vertices affected by the merge are added or removed from the stacks as appropriate.
                PushOnStacksIfConditionsMet(v.Neighbors.Where(x => x.Id != w.Id).Union(u.Neighbors));
            }

            // At this point S4, S5, and the graph are empty. We can color the graph.
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
                else if (v.Degree == 5 && v.Neighbors.Any(n => n.Degree <= 6))
                    _s5.Push(v);
            }
        }

        private int[] ColorRemoved()
        {
            var coloring = new int[_vs.Length];

            while (_sd.Count > 0)
            {
                // We pop vertices off Sd.
                var v = _sd.Pop();

                // If the vertex were merged with another vertex, 
                // the vertex that it was merged with will already have been colored, and we assign it the same color.
                // This is valid because we only merged vertices that were not adjacent in the original graph
                if (v.IsMerged)
                {
                    coloring[v.Id] = coloring[v.MergedWith];
                }
                // Otherwise vertex had degree 4 at the point of time of its removal
                // and we can simply assign it a color none of its neighbors has
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
