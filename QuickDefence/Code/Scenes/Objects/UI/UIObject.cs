using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

#nullable enable

namespace Scenes.Objects.UI
{
    internal class UIElement : GameObject
    {
        // I'm looking at this and just wondering wh-
        // Oh of course, no texture because text and buttons lol, riiiiight... Yeah, that was a hack.
        // This class is literally just GameObject named differently for sorting purposes... It could literally have just been a boolean
        // BUT COME ON, I DIDN'T MAKE EITHER OF THOSE UNTIL I THINK THE END OF DAY 2!!!
        public UIElement(Texture2D? texture, Vector2 position = new Vector2(), int layer = 0, float rotation = 0, Vector2 size = new Vector2(), float alpha = 1) : base(position, layer, rotation, size, alpha)
        {
            if(texture != null) _texture = texture;

            if (this.GetType() == typeof(UIElement)) Debug.WriteLine("UIElement initialized.");
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            base.Render(ref sb, window);
        }

        public override void RenderRelative(ref SpriteBatch sb, GameWindow window, Vector2 relativeTo)
        {
            base.RenderRelative(ref sb, window, relativeTo);
        }
    }
}
