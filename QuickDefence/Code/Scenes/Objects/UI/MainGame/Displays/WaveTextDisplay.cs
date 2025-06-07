using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scenes.Objects.MainGame.Spawner;
using Scenes.Objects.UI.Text;
using Scenes.SceneTypes;
using System;
using System.Diagnostics;

namespace Scenes.Objects.UI.MainGame.Displays
{
    internal class WaveTextDisplay : TextDisplay
    {
        public WaveTextDisplay(SpriteFont font, string defaultTextValue, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default) 
            : base(font, defaultTextValue, position, layer, rotation, size)
        {
            _textValue = DEFAULT_TEXT + "1"; // Ooooo yeaaaah


            SetupDelegates();

            if (this.GetType() == typeof(WaveTextDisplay)) Debug.WriteLine("Wave Display initialized.");
        }

        ~WaveTextDisplay()
        {
            RemoveDelegates();
        }

        const string DEFAULT_TEXT = "Round: "; // Wave text... Round... Yeeeee, I'm starting to think I was high making this


        // Now these are delegates fuck yeah
        // All static, I love it
        // I am totally doing them all like this from now on
        void SetupDelegates()
        {
            EnemySpawner.onWaveStarted += UpdateTextOnWaveStart;

            MainGameScene._onWin += UpdateTextOnWin;
            MainGameScene._onLoss += UpdateTextOnLoss;
        }

        void RemoveDelegates()
        {
            EnemySpawner.onWaveStarted -= UpdateTextOnWaveStart;

            MainGameScene._onWin -= UpdateTextOnWin;
            MainGameScene._onLoss -= UpdateTextOnLoss;
        }

        public void UpdateTextOnWaveStart(object sender, EventArgs e)
        {
            int wave = (sender as EnemySpawner)._wave;

            _textValue = (wave == -1) ? "You win yippeee" : DEFAULT_TEXT + wave.ToString();
        }

        public void UpdateTextOnWin(object sender, EventArgs e)
        {
            _textValue = "You WIN!. [Press Escape to exit.]";
        }

        public void UpdateTextOnLoss(object sender, EventArgs e)
        {
            _textValue = "You Lose. [Press Escape to exit.]";
        }
    }
}
