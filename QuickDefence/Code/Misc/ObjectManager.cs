using Microsoft.Xna.Framework;
using Scenes.Objects;
using Scenes.Objects.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Misc
{
    public static class ObjectManager
    {
        public static List<GameObject> _objects = new(); // should not be public
                                                         // should be handled using setters


        public static bool _dirtyLayer; // should not be public
                                        // should be ha- you get the idea

        public static EventHandler<OnGameObjectCreatedEventArgs> onGameObjectCreated;
        public class OnGameObjectCreatedEventArgs : EventArgs
        {
            public GameObject obj;
        }

        public static EventHandler onTowerPreviewCreated, onTowerPreviewDestroyed;

        public static void OnDirtyLayer()
        {
            _objects = _objects.OrderBy(o => o is UIElement ? 1 : 0)
                    .ThenBy(o => o._layer)
                    .ToList();

            _dirtyLayer = false;
        }

        // Aww I remember I was excited to use this as well
        public static bool ValidateObjectIsPopulated(object obj)
        {
            // Get both fields and properties of the object (public and private)
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            PropertyInfo[] properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            // Check fields
            foreach (var field in fields)
            {
                if (field.GetValue(obj) == null)
                {
                    return false;
                }
            }

            // Check properties
            foreach (var property in properties)
            {
                if (property.GetValue(obj) == null)
                {
                    return false;
                }
            }

            return true;
        }

        // AABB moment
        public static bool GetObjectOverlap(GameObject obj1, GameObject obj2)
        {
            return obj1._position.X                < obj2._position.X + obj2._size.X &&
                   obj1._position.X + obj1._size.X > obj2._position.X &&

                   obj2._position.Y                < obj1._position.Y + obj1._size.Y &&
                   obj2._position.Y + obj2._size.Y > obj1._position.Y;
        }


        // Woooo another unused method
        public static float GetDistance(GameObject obj1, GameObject obj2)
        {
            Vector2 pos1 = obj1._position;
            Vector2 pos2 = obj2._position;

            return MathF.Sqrt(MathF.Pow(pos1.X - pos2.X, 2) + MathF.Pow(pos1.Y - pos2.Y, 2));
        }

        public static float GetDistanceFromOrigin(GameObject obj1, GameObject obj2)
        {
            Vector2 pos1 = obj1._position + obj1._size / 2;
            Vector2 pos2 = obj2._position + obj2._size / 2;

            return MathF.Sqrt(MathF.Pow(pos1.X - pos2.X, 2) + MathF.Pow(pos1.Y - pos2.Y, 2));
        }
    }
}
