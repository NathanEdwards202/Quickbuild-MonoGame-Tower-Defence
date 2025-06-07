using Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scenes.Objects.UI.MainGame.Displays;
using Scenes.Objects.UI.Text;
using System;
using System.Diagnostics;

#nullable enable

// See WaveTextDisplay.cs for comments :)
// This is basically the same class

namespace Scenes.Objects.UI.MainGame.Displays
{
    internal class HealthDisplay : TextDisplay
    {
        public HealthDisplay(SpriteFont font, string? defaultTextValue, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default)
            : base(font, defaultTextValue, position, layer, rotation, size)
        {
            _textValue = DEFAULT_TEXT + Player._lives.ToString();


            SetupDelegates();

            if (this.GetType() == typeof(HealthDisplay)) Debug.WriteLine("Health Display initialized.");
        }

        ~HealthDisplay()
        {
            RemoveDelegates();
        }

        const string DEFAULT_TEXT = "Lives: ";

        void SetupDelegates()
        {
            Player.onEnemyReachedEnd += UpdateTextValueOnEnemyReachedEnd;
        }

        void RemoveDelegates()
        {
            Player.onEnemyReachedEnd -= UpdateTextValueOnEnemyReachedEnd;
        }

        public void UpdateTextValueOnEnemyReachedEnd(object? sender, EventArgs e)
        {
            _textValue = DEFAULT_TEXT + Player._lives.ToString();
        }
    }
}
