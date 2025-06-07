using Misc;
using QuickDefence.Code.Scenes.Objects.UI.MainGame.SideBar;
using Scenes.Objects.UI.Interactable.Buttons;
using Scenes.Objects.UI.MainGame.Displays;
using Scenes.Objects.UI.MainGame.SideBar;
using System.Collections.Generic;

namespace Scenes.Objects.UI.MainGame
{
    // I'm really not sure about having this as a static class
    // This does not need to be in memory except at the start of a maingamescene
    internal static class MainGameUICreator
    {
        // Make that shiiiiiiiit
        // Gets passed into Scene.AddToScene()
        public static List<UIElement> GenerateSceneUIElements()
        {
            return new List<UIElement>
            {
                CreateInfoPanel(),
                CreateWaveDisplay(),
                CreateHealthDisplay(),
                CreateNextRoundButton(),
                CreateTowerButtons(0, TowerHolder._towers["Regular Tower"]),
                CreateTowerButtons(1, TowerHolder._towers["Knight"]),
                CreateTowerButtons(2, TowerHolder._towers["Wizard"]),
                CreateTowerButtons(3, TowerHolder._towers["Pengu"])
            };
        }


        // Info Panel Background
        const int BG_WIDTH = 1420;
        const int BG_HEIGHT = 1080;
        const int BG_INFO_WIDTH = 500;

        static UIElement CreateInfoPanel()
        {
            UIElement infoPanel = new(AssetManager._textures["InfoBG"]
                , position: new(BG_WIDTH, 0)
                , layer: -1 // Get this behind everything
                , rotation: 0
                , size: new(BG_INFO_WIDTH, BG_HEIGHT));
            return infoPanel;
        }


        // Wave Display
        const int WAVE_POSITION_X = 1440;
        const int WAVE_POSITION_Y = 20;
        const int WAVE_SIZE_X = 440;
        const int WAVE_SIZE_Y = 100;

        static WaveTextDisplay CreateWaveDisplay()
        {
            WaveTextDisplay newWaveDisplay = new(
                font: AssetManager.GetFont("DefaultFontArial"),
                defaultTextValue: "",
                position: new(WAVE_POSITION_X, WAVE_POSITION_Y),
                layer: 0,
                rotation: 0,
                size: new(WAVE_SIZE_X, WAVE_SIZE_Y)
                );

            return newWaveDisplay;
        }


        // Health Display
        const int HEALTH_DISPLAY_POSITION_X = 1440;
        const int HEALTH_DISPLAY_POSITION_Y = 140;
        const int HEALTH_DISPLAY_SIZE_X = 440;
        const int HEALTH_DISPLAY_SIZE_Y = 100;

        static HealthDisplay CreateHealthDisplay()
        {
            HealthDisplay newHealthDisplay = new(
                font: AssetManager.GetFont("DefaultFontArial"),
                defaultTextValue: "",
                position: new(HEALTH_DISPLAY_POSITION_X, HEALTH_DISPLAY_POSITION_Y),
                layer: 0,
                rotation: 0,
                size: new(HEALTH_DISPLAY_SIZE_X, HEALTH_DISPLAY_SIZE_Y)
                );

            return newHealthDisplay;
        }


        // Next Round Button
        const int NEXT_ROUND_BUTTON_SIZE_X = 240;
        const int NEXT_ROUND_BUTTON_SIZE_Y = 240;
        const int NEXT_ROUND_BUTTON_POSITION_X = 1920 - NEXT_ROUND_BUTTON_SIZE_X - 20;
        const int NEXT_ROUND_BUTTON_POSITION_Y = 1080 - NEXT_ROUND_BUTTON_SIZE_Y - 20;

        static NextRoundButton CreateNextRoundButton()
        {
            ButtonStateRenderingHolder interactableStateRenderingHolder, hoveringStateRenderingHolder, heldStateRenderingHolder, pressedStateRenderingHolder, deactivatedStateRenderingHolder;

            interactableStateRenderingHolder = new(
                texture: AssetManager.GetTexture("PlayButtonInteractable")
                );
            hoveringStateRenderingHolder = new(
                texture: AssetManager.GetTexture("PlayButtonHovering")
                );
            heldStateRenderingHolder = new(
                texture: AssetManager.GetTexture("PlayButtonHeld")
                );
            pressedStateRenderingHolder = new(
                texture: AssetManager.GetTexture("PlayButtonPressed")
                );
            deactivatedStateRenderingHolder = new(
                texture: AssetManager.GetTexture("PlayButtonDisabled"),
                alpha: 0.5f
                );

            NextRoundButton nextRoundButton = new(
                interactableHolder: interactableStateRenderingHolder,
                hoveringHolder: hoveringStateRenderingHolder,
                heldHolder: heldStateRenderingHolder,
                pressedHolder: pressedStateRenderingHolder,
                deactivatedHolder: deactivatedStateRenderingHolder,
                defaultState: ButtonState.Interactable,
                position: new(
                    NEXT_ROUND_BUTTON_POSITION_X,
                    NEXT_ROUND_BUTTON_POSITION_Y
                    ),
                layer: 0,
                rotation: 0,
                size: new(
                    NEXT_ROUND_BUTTON_SIZE_X,
                    NEXT_ROUND_BUTTON_SIZE_Y
                    )
                );

            return nextRoundButton;
        }


        // Tower Buttons
        const int TOWER_BUTTON_SIZE = 128;
        const int TOWER_BUTTON_POSITION_LEFT_X = 1920 - 500 + 50;
        const int TOWER_BUTTON_POSITION_RIGHT_X = 1920 - 50 - TOWER_BUTTON_SIZE;
        const int TOWER_BUTTON_POSITION_TOP_Y = 260;
        const int TOWER_BUTTON_POSITION_BOTTOM_Y = TOWER_BUTTON_POSITION_TOP_Y + 50 + TOWER_BUTTON_SIZE;

        static TowerButton CreateTowerButtons(int buttonNumber, TowerStatsTemplate template)
        {
            if (buttonNumber > 3) throw new System.Exception("Only 4 towers allowed at once");

            bool left = buttonNumber % 2 == 0;
            bool top = buttonNumber - 2 < 0;

            TowerButton button = new(
                towerTemplate: template,
                interactableHolder: new(
                    texture: AssetManager.GetTexture(template._textureName),
                    1
                    ),
                position: new(
                    left ? TOWER_BUTTON_POSITION_LEFT_X : TOWER_BUTTON_POSITION_RIGHT_X,
                    top ? TOWER_BUTTON_POSITION_TOP_Y : TOWER_BUTTON_POSITION_BOTTOM_Y
                    ),
                size: new(
                    TOWER_BUTTON_SIZE,
                    TOWER_BUTTON_SIZE
                    )
                );

            return button;
        }
    }
}
