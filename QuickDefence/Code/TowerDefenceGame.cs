using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Scenes;
using Scenes.SceneTypes;
using System.Diagnostics;


namespace QuickDefence
{
    internal class TowerDefenceGame
    {
        public TowerDefenceGame()
        {
            // If this was a proper game, current scene would be set to a title screen lol
            // I didn't even make a switch scene method, that's why this file is so bare
            _currentScene = new MainGameScene("Level1Scene.json");

            Debug.WriteLine("Game initialized.");
        }

        Scene _currentScene; // Visual studio wants me to make this readonly... But like... If this was made properly it wouldn't be

        public void Update(GameTime gameTime)
        {
            _currentScene.Update(gameTime);
        }

        public void Render(ref SpriteBatch sb, GameWindow window)
        {
            _currentScene.Render(ref sb, window);
        }
    }
}