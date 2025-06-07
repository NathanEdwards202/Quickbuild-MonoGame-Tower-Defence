using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

#nullable enable

namespace Scenes.Objects.UI.Text
{
    // I'd like to think this class is self-explanatory enough
    internal class TextDisplay : UIElement
    {
        public TextDisplay(SpriteFont font, string? defaultTextValue, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default) 
            : base(null, position, layer, rotation, size)
        {
            _font = font;
            if (defaultTextValue == null) _textValue = "";

            else _textValue = defaultTextValue;

            if (this.GetType() == typeof(TextDisplay)) Debug.WriteLine("TextDisplay initialized.");
        }

        protected SpriteFont _font;
        public string _textValue { get; protected set; }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            Vector2 textSize = _font.MeasureString(_textValue);
            Vector2 scale = new(_size.X / textSize.X, _size.Y / textSize.Y);
            Vector2 origin = new(textSize.X / 2f, textSize.Y / 2f);

            sb.DrawString(
                spriteFont: _font,
                text: _textValue,
                position: new(
                    (int)_position.X + (int)(origin.X * scale.X),
                    (int)_position.Y + (int)(origin.Y * scale.Y)
                    ),
                color: Color.Black,
                rotation: 0f,
                origin: origin,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
    }
}
