using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using Scenes.Objects.MainGame.Paths;
using System;
using System.Collections.Generic;

namespace Scenes.Objects.MainGame.PlacementBlockers
{
    internal class PlacementBlocker : GameObject
    {
        public PlacementBlocker(Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default, float alpha = 1)
            : base(position, layer, rotation, size, alpha)
        {
            _texture = AssetManager.GetTexture("Pixel");
        }

        const int ENEMY_SIZE_OFFSET = 32;

        // GPT GAVE ME THIS IN A COMPLETELY BROKEN WAY
        // THE AMOUNT OF CHANGES I HAD TO MAKE OH MY GOD THAT THING DROVE ME INSANE
        public static List<PlacementBlocker> GeneratePlacementBlockersFromPaths(List<Path> paths, int pathWidth = 86)
        {
            List<PlacementBlocker> result = new();

            List<PathPointConnection> connections = IdentifyUniqueConnections(paths);

            foreach(PathPointConnection connection in connections) // Loop until the second-to-last point
            {
                Vector2 pos1 = connection._point1._position;
                Vector2 pos2 = connection._point2._position;

                // Enemy size offset correction
                pos1 = new(
                    pos1.X + ENEMY_SIZE_OFFSET,
                    pos1.Y + ENEMY_SIZE_OFFSET
                    );

                pos2 = new(
                    pos2.X + ENEMY_SIZE_OFFSET,
                    pos2.Y + ENEMY_SIZE_OFFSET
                    );

                // Direction vector from pos1 to pos2
                Vector2 dir = pos2 - pos1;
                dir.Normalize();

                // Perpendicular vector to the path direction
                Vector2 perp = new(-dir.Y, dir.X);

                // Half of the total width to distribute on both sides of the path
                float halfWidth = pathWidth / 2.0f;

                // Calculate the four corners of the rectangle
                // Maybe actually take dir into account next time gpt
                Vector2 corner1 = pos1 + perp * halfWidth - dir * halfWidth;
                Vector2 corner2 = pos1 - perp * halfWidth + dir * halfWidth;
                Vector2 corner3 = pos2 + perp * halfWidth - dir * halfWidth;
                Vector2 corner4 = pos2 - perp * halfWidth + dir * halfWidth;

                // Calculate the top-left corner of the rectangle (minimum x and y of the corners)
                float minX = Math.Min(Math.Min(corner1.X, corner2.X), Math.Min(corner3.X, corner4.X));
                float minY = Math.Min(Math.Min(corner1.Y, corner2.Y), Math.Min(corner3.Y, corner4.Y));
                Vector2 topLeft = new(minX, minY);

                // Calculate the width and height of the rectangle
                float maxX = Math.Max(Math.Max(corner1.X, corner2.X), Math.Max(corner3.X, corner4.X));
                float maxY = Math.Max(Math.Max(corner1.Y, corner2.Y), Math.Max(corner3.Y, corner4.Y));
                Vector2 size = new(maxX - minX, maxY - minY); // Maybe actually calculate this properly next time GPT


                // Maybe don't use corner1, maxX as the values next time GPT
                PlacementBlocker blocker = new(
                    position: topLeft,
                    size: size
                    );

                result.Add(blocker);
            }

            return result;
        }

        // This, on the other hand
        // Thanks GPT, worked first time
        static List<PathPointConnection> IdentifyUniqueConnections(List<Path> paths)
        {
            HashSet<(ushort, ushort)> uniqueConnections = new(); // To keep track of unique connections
            List<PathPointConnection> result = new(); // To store the actual connections

            foreach (Path path in paths)
            {
                // Ensure the path is sorted by point ID to have consistent ordering of connections
                path.SortPath();

                // Iterate through each consecutive pair of points in the path
                for (int i = 0; i < path._points.Count - 1; i++)
                {
                    PathPoint point1 = path._points[i];
                    PathPoint point2 = path._points[i + 1];

                    // Create a tuple representing the connection (smaller ID first to avoid duplicates in reverse order)
                    (ushort, ushort) connectionKey = point1._pointID < point2._pointID
                        ? (point1._pointID, point2._pointID)
                        : (point2._pointID, point1._pointID);

                    // If this connection has not been seen before, add it to the result and mark it as seen
                    if (!uniqueConnections.Contains(connectionKey))
                    {
                        uniqueConnections.Add(connectionKey);
                        result.Add(new PathPointConnection(point1, point2));
                    }
                }
            }

            return result;
        }

        // Oh yeah, this was a first attempt to brute-force correct GPTs fuckup
        // It did not work
        // Forgor to remove
        static bool CheckHorizontal(Vector2 perp)
        {
            // A vector is considered horizontal if its Y-component is close to zero.
            // You may adjust the tolerance based on the precision needed.
            const float tolerance = 0.01f;

            return Math.Abs(perp.Y) < tolerance;
        }


#if DEBUG
        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            sb.Draw(
                texture: _texture,
                destinationRectangle: new Rectangle(
                    (int)_position.X,
                    (int)_position.Y,
                    (int)_size.X,
                    (int)_size.Y
                ),
                sourceRectangle: null,
                color: Color.Yellow * 0.25f,
                rotation: 0,
                origin: Vector2.Zero, // Why zero and not the centre like usual? Fuck knows, but this makes it work :)
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
#endif
    }
}