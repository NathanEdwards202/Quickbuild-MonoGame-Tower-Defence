using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Numerics;


// Wow what an amazing, totally necessary file

namespace Scenes.Objects.MainGame.Background
{
    internal class BackgroundImage : GameObject
    {
        public BackgroundImage(Texture2D texture, Vector2 size) : base(new(0, 0), -1, 0, size)
        {
            _texture = texture;

            if (this.GetType() == typeof(BackgroundImage)) Debug.WriteLine("Background Image initialized.");
        }
    }
}