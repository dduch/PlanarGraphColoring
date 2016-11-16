namespace PlanarGraphColoring
{
    public class RemovedVertex
    {
        public int Id { get; }
        public int MergedWith { get; }
        public bool IsMerged
        {
            get
            {
                return MergedWith >= 0;
            }
        }
        public int[] Neighbors { get; }

        public RemovedVertex(int id, int[] neighbors, int mergedWith = -1)
        {
            Id = id;
            Neighbors = neighbors;
            MergedWith = mergedWith;
        }
    }
}
