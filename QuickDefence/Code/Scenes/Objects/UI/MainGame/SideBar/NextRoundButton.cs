using Microsoft.Xna.Framework;
using Scenes.Objects.MainGame.Spawner;
using Scenes.Objects.UI.Interactable.Buttons;
using System;
using System.Diagnostics;

namespace Scenes.Objects.UI.MainGame.SideBar
{
    internal class NextRoundButton : Button
    {
        public NextRoundButton(ButtonStateRenderingHolder interactableHolder, 
            ButtonStateRenderingHolder? hoveringHolder = null, ButtonStateRenderingHolder? heldHolder = null, ButtonStateRenderingHolder? pressedHolder = null, ButtonStateRenderingHolder? deactivatedHolder = null, 
            ButtonState defaultState = ButtonState.Interactable, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default) 
            : base(interactableHolder, hoveringHolder, heldHolder, pressedHolder, deactivatedHolder, defaultState, position, layer, rotation, size)
        {
            SetupDelegates();
            if (this.GetType() == typeof(NextRoundButton)) Debug.WriteLine("Next round button initialized.");
        }

        ~NextRoundButton()
        {
            RemoveDelegates();
            Dispose();
        }

        public override void Dispose()
        {
            base.Dispose();
            buttonClicked = null; // I really need to standardize my way of doing this huh
                                  // Devnote... I did not standardize my way of doing this... I wish I did.
        }

        public static new event EventHandler buttonClicked;

        void SetupDelegates()
        {
            EnemySpawner.onWaveStarted += DisableOnWaveStart;
            EnemySpawner.onWaveEnded += EnableOnWaveEnd;
        }

        void RemoveDelegates()
        {
            EnemySpawner.onWaveStarted -= DisableOnWaveStart;
            EnemySpawner.onWaveEnded -= EnableOnWaveEnd;
        }

        public override void OnClick()
        {
            buttonClicked?.Invoke(this, EventArgs.Empty);
        }

        void EnableOnWaveEnd(object sender, EventArgs e) // Awfully named function... EnableOnAllEnemiesSpawned would be more accurate
        {
            ActivateButton();
        }

        void DisableOnWaveStart(object sender, EventArgs e)
        {
            DeactivateButton();
        }
    }
}
