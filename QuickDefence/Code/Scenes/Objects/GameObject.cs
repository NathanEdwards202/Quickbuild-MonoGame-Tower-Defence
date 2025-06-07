using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;

#nullable enable

namespace Scenes.Objects
{
    public class GameObject
    {
        // Constructor and Initializer
        // This constructor is a cause of nightmares. Everything that derives from it has such a long constructor... I really should've made a struct
        public GameObject(Vector2 position = new Vector2(), int layer = 0, float rotation = 0, Vector2 size = new Vector2(), float alpha = 1)
        {
            _position = position;
            _rotation = rotation;
            _layer = layer;
            _size = size;
            _alpha = alpha;

            _forDeletion = false;

            if (this.GetType() == typeof(GameObject)) Debug.WriteLine("Object initialized.");
        }


        public Vector2 _position { get; protected set; }
        public float _rotation { get; protected set; }
        public int _layer { get; protected set; }

        public Vector2 _size { get; protected set; }

        public bool _forDeletion { get; protected set; } // Used to destroy the fucker at the end of the frame
                                                         // This is kind of a poor design choice, had to make sure that anytime this got set to true whatever it was doing ends immediately.
                                                         // I honestly have no idea if I forgot to do this somewhere or other


        public Texture2D? _texture { get; protected set; }
        public float _alpha { get; protected set; } // You have no idea how many times I kept going like Wait... Alpha does mean that thing for transparency right?
                                                    // (I do not have the heart to check now)


        // Seeing this empty now, I realize this should've been an abstract class.
        public virtual void Update(GameTime gameTime)
        {

        }

        // I physically hate the sheer amount of casts to int here, but ehhhhh, it's not been a problem
        public virtual void Render(ref SpriteBatch sb, GameWindow window)
        {
            if (_texture == null) return; // And this is why texture is nullable

            Vector2 origin = new(_size.X / 2f, _size.Y / 2f);

            sb.Draw(
                texture: _texture,
                destinationRectangle: new Rectangle(
                        (int)_position.X + (int)origin.X, 
                        (int)_position.Y + (int)origin.Y, 
                        (int)_size.X, 
                        (int)_size.Y
                        ),
                sourceRectangle: null,
                color: Color.White * _alpha,
                rotation: _rotation,
                origin: origin,
                effects: SpriteEffects.None,
                layerDepth: 0
                );
        }

        public virtual void RenderRelative(ref SpriteBatch sb, GameWindow window, Vector2 relativeTo)
        {
            if (_texture == null) return;

            Vector2 origin = new(_size.X / 2f, _size.Y / 2f);

            sb.Draw(
                texture: _texture,
                destinationRectangle: new Rectangle(
                        (int)relativeTo.X + (int)_position.X + (int)origin.X,
                        (int)relativeTo.Y + (int)_position.Y + (int)origin.Y,
                        (int)_size.X,
                        (int)_size.Y
                        ),
                sourceRectangle: null,
                color: Color.White * _alpha,
                rotation: _rotation,
                origin: origin,
                effects: SpriteEffects.None,
                layerDepth: 0
                );
        }


        // Helper function that I really thought I'd use more
        // And I am also 99% sure this code is rewritten in the tower logic somewhere
        // I think I just forgot I had this here
        protected static float GetRadiansFromNormalizedVector(Vector2 normalizedVector)
        {
            float radians = MathF.Atan2(normalizedVector.Y, normalizedVector.X);

            // Normalize angle to [0, 2pi]
            if (radians < 0) radians += MathF.PI * 2;

            return radians;
        }
    }
}
