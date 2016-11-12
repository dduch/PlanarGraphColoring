using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanarGraphColoring
{
    class Vertex
    {
        public int Id { get; }
        public int Degree
        {
            get
            {
                return Neighbors.Count();
            }
        }
        public IEnumerable<Vertex> Neighbors { get; set; }
        public bool IsRemoved { get; private set; }
        public bool ToBeRemoved { get; set; }

        public Vertex(int id)
        {
            Id = id;
            IsRemoved = false;
            ToBeRemoved = false;
        }

        public bool IsAdjacentTo(Vertex v)
        {
            return Neighbors.Any(n => n.Id == v.Id);
        }

        public RemovedVertex RemoveFromGraph()
        {
            IsRemoved = true;
            foreach (var n in Neighbors)
                n.Neighbors = n.Neighbors.Where(x => x.Id != Id);
            return new RemovedVertex(Id, Neighbors.Select(x => x.Id).ToArray());
        }

        // Merges 'toMerge' vertex into this vertex on 'mergeOn' vertex and removes both 'toMerge' and 'mergeOn' form graph
        public Tuple<RemovedVertex,RemovedVertex> MergeWithAndRemove(Vertex toMerge, Vertex mergeOn)
        {
            var l1 = Neighbors.ToList();
            var l2 = toMerge.Neighbors.ToList();

            var i1 = l1.IndexOf(mergeOn);
            var i2 = l2.IndexOf(mergeOn);

            var l1top = l1.Take(i1);
            var l1btm = l1.Skip(i1);
            var l2top = l2.Take(i2);
            var l2btm = l2.Skip(i2);

            // Vertex becomes adjacent to all neighbors of 'toMerge' preserving planar order
            Neighbors = l1btm.Union(l1top).Union(l2btm).Union(l2top).Distinct();

            // Now we can safely remove 'mergeOn' from graph
            var r1 = mergeOn.RemoveFromGraph();

            // We must update adjacency list of all former neighbors of 'toMerge' vertex
            foreach (var mergedNeighbor in toMerge.Neighbors)
            {
                var n = mergedNeighbor.Neighbors;
                if (n.Contains(this))
                    n = n.Where(x => x.Id != toMerge.Id);
                else
                    n = n.Select(x => x.Id == toMerge.Id ? this : x);

                mergedNeighbor.Neighbors = n;
            }

            toMerge.IsRemoved = true;
            var r2 = new RemovedVertex(toMerge.Id, toMerge.Neighbors.Select(x => x.Id).ToArray(), Id);

            return new Tuple<RemovedVertex, RemovedVertex>(r1, r2);
        }
    }
}
