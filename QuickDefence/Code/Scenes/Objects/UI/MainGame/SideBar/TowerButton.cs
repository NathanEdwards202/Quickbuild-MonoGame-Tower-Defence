using Microsoft.Xna.Framework;
using Misc;
using Scenes.Objects.UI.Interactable.Buttons;
using Scenes.Objects.UI.MainGame.Actors.Characters;
using System;

namespace QuickDefence.Code.Scenes.Objects.UI.MainGame.SideBar // EWWWW I FORGOT TO REMOVE QUICKDEFENCE.CODE
{
    internal class TowerButton : Button
    {
        // Holy mother of huge constructor size
        public TowerButton(TowerStatsTemplate towerTemplate, ButtonStateRenderingHolder interactableHolder, 
            ButtonStateRenderingHolder? hoveringHolder = null, ButtonStateRenderingHolder? heldHolder = null, ButtonStateRenderingHolder? pressedHolder = null, ButtonStateRenderingHolder? deactivatedHolder = null, 
            ButtonState defaultState = ButtonState.Interactable, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default) 
            : base(interactableHolder, hoveringHolder, heldHolder, pressedHolder, deactivatedHolder, defaultState, position, layer, rotation, size)
        {
            _towerTemplate = towerTemplate;

            SetupDelegates();
        }

        ~TowerButton()
        {
            RemoveDelegates();
        }

        void SetupDelegates()
        {
            buttonClicked += OnClick;
            ObjectManager.onTowerPreviewCreated += DeactivateOnTowerPreviewCreated;
            ObjectManager.onTowerPreviewDestroyed += ActivateOnTowerPreviewDestroyed;
        }

        void RemoveDelegates()
        {
            buttonClicked -= OnClick;
            ObjectManager.onTowerPreviewCreated -= DeactivateOnTowerPreviewCreated;
            ObjectManager.onTowerPreviewDestroyed -= ActivateOnTowerPreviewDestroyed;
        }

        TowerStatsTemplate _towerTemplate;

        void OnClick(object sender, EventArgs e)
        {
            TowerPreview preview = new(
                activatedFrom: this,
                tower: new(
                    stats: new(
                        template: _towerTemplate
                        )
                    ),
                size: new( // Hard-code dat siiiiiiize
                    128,
                    128
                    )
                );

            ObjectManager.onGameObjectCreated?.Invoke(null, new ObjectManager.OnGameObjectCreatedEventArgs { obj = preview });
        }

        void DeactivateOnTowerPreviewCreated(object sender, EventArgs e)
        {
            DeactivateButton();
        }

        void ActivateOnTowerPreviewDestroyed(object sender, EventArgs e)
        {
            ActivateButton();
        }
    }
}