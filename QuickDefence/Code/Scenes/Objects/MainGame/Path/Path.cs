using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace Scenes.Objects.MainGame.Paths
{
    internal class Path
    {
        public Path(List<PathPoint> pathPoints)
        {
            _points = pathPoints;
            Debug.WriteLine($"Path initialized with {pathPoints.Count} points.");
        }

        public List<PathPoint> _points { get; private set; } // Use private set for deserialization
                                                             // This comment is from when I tried a completely different implementation using gpt which did not work at all

        public void SortPath()
        {
            _points = _points.OrderBy(p => p._pointID).ToList();
        }

        // Ughhhh I knew I had a better way to do all that in the enemy class
        // I did this manually
        public bool CheckEndOfTrack(int currentPoint)
        {
            return _points.Count < currentPoint + 1;
        }
    }

    internal class PathPoint : GameObject
    {
        public PathPoint(ushort pointID, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default)
            : base(position, layer, rotation, size)
        {
            _pointID = pointID;
            if (this.GetType() == typeof(PathPoint)) Debug.WriteLine("Path Point initialized.");
        }

        [JsonInclude] // This tag is also from when I tried a completely different implementation using gpt which did not work at all
        public ushort _pointID { get; private set; }
    }

    struct PathPointConnection
    {
        public PathPointConnection(PathPoint point1, PathPoint point2)
        {
            _point1 = point1;
            _point2 = point2;
        }

        public PathPoint _point1 { get; private set; }
        public PathPoint _point2 { get; private set; }
    }
}
