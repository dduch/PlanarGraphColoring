using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColoringTests
{
    class PlantriGraphLoader
    {
        private readonly FileStream _stream;
        private readonly int _verticesCount;

        public PlantriGraphLoader(string filename)
        {
            _stream = new FileStream(filename, FileMode.Open);
            _stream.Seek(15, SeekOrigin.Begin);
            _verticesCount = _stream.ReadByte();

        }

        public int[][] LoadGraph()
        {
            if (_stream.Position == _stream.Length)
            {
                _stream.Close();
                return null;
            }

            var graph = new int[_verticesCount][];
            for (int i = 0; i < _verticesCount; ++i)
            {
                var neighbors = new List<int>();
                var b = _stream.ReadByte();
                while (b != 0)
                {
                    neighbors.Add(b - 1);
                    b = _stream.ReadByte();
                }
                graph[i] = neighbors.ToArray();
            }
            _stream.ReadByte(); // Advance stram position by 1
            return graph;
        }
    }
}
