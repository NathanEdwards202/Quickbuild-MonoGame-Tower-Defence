using Controllers.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace Scenes.Objects.UI.Interactable
{
    // Abstractable
    internal class ClickableUIElement : UIElement
    {
        public ClickableUIElement(Texture2D texture, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default, float alpha = 1)
            : base(texture, position, layer, rotation, size, alpha)
        {


            if (this.GetType() == typeof(ClickableUIElement)) Debug.WriteLine("ClickableUIElement initialized.");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public bool GetMouseOverlap()
        {
            if (!MouseController.GetButtonPressed(MouseButton.Left)) return false; // Why are you here?
                                                                                   // This is an overlap check not a click check?
                                                                                   // IN NO CIRCUMSTANCES SHOULD THIS WORK
                                                                                   // BUT IT DOES
                                                                                   // AND I DO NOT KNOW WHY

            if (MouseController.GetMouseOverlap(this)) return true;
            else return false;
        }

        public virtual void OnClick()
        {

        }

        // I noticed I put these overrides and just left them base a lot huh
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