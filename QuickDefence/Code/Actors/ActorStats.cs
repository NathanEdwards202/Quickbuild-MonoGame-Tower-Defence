using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Actors
{
    internal class ActorStats
    {
        public ActorStats(string name
            , Texture2D texture
            , float rotationSpeed)
        {
            _name = name;
            _texture = texture;
            _rotationSpeed = rotationSpeed / 180f * MathF.PI; // Multiplication does radian crap
            // I am almost certain there are other stats all actors have but I cba to check :)

            if (this.GetType() == typeof(ActorStats)) Debug.WriteLine("ActorStats initialized.");
        }

        public string _name { get; protected set; }

        public Texture2D _texture {  get; protected set; }
        public float _rotationSpeed {get; protected set;}
    }
}