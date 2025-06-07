using Actors;
using Microsoft.Xna.Framework;
using Scenes.Objects;
using System;
using System.Diagnostics;

namespace QuickDefence.Code.Actors
{
    internal class Actor : GameObject
    {
        public Actor(string name, ActorStats stats, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default) 
            : base(position, layer, rotation, size)
        {
            _name = name;
            _stats = stats;

            if (this.GetType() == typeof(Actor)) Debug.WriteLine($"Actor {_name} initialize");
        }

        public string _name { get; private set; }
        public ActorStats _stats { get; protected set; }

        protected void DoRotation(GameTime gameTime, float targetRotation)
        {
            if (_rotation == targetRotation && _name != "ScurryBug") return;

            // Calculate the difference between the target and the current rotation
            float diff = targetRotation - _rotation;

            // If the difference is greater than pi, rotate in the opposite direction to minimize the rotation path
            if (MathF.Abs(diff) > MathF.PI)
            {
                // Rotate counter-clockwise if the difference is large enough to cross the boundary
                if (diff > 0)
                {
                    diff -= MathF.PI * 2; // Subtract 2pi for counter-clockwise rotation
                }
                else
                {
                    diff += MathF.PI * 2; // Add 2pi for clockwise rotation
                }
            }

            // Adjust the change rate based on the direction of the difference
            float change = _stats._rotationSpeed * MathF.Sign(diff); // Sign returns 1 for positive, -1 for negative
            change *= (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateRotation(change);

            if (MathF.Abs(targetRotation - _rotation) < MathF.PI / 24f && _name != "ScurryBug") _rotation = targetRotation; // If scurrybug, make sure it always overshoots the rotation for funny "animation"
        }

        // Helper function to allow counter-clockwise rotation
        public void UpdateRotation(float change)
        {
            _rotation += change;

            if (_rotation >= MathF.PI * 2)
            {
                _rotation -= MathF.PI * 2;
            }

            else if (_rotation < 0)
            {
                _rotation += MathF.PI * 2;
            }
        }
    }
}
