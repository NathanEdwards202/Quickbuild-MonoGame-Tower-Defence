using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Controllers.Inputs;
using System.Diagnostics;
using System;
using System.IO;
using Misc;

namespace QuickDefence
{
    public class Game1 : Game
    {
        // Template code
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Game
        TowerDefenceGame _thisGame;

        // Textures
        public static Dictionary<string, Texture2D> _textures {get; private set;}

        // Template code
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        // Early Initialize, anything not requiring textures
        protected override void Initialize()
        {
            // Set monitor shit
            _graphics.HardwareModeSwitch = false;
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            // Initialize mouse inputs
            // Keyboard doesn't need initialization because monogame is inconsistant
            MouseController.Initialize();

            // Initialize TextureManager
            Misc.AssetManager.SetContentManger(Content, GraphicsDevice);

            // Keep last
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Template code
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            try
            {
                // Generate a 1x1 pixel (stretched for various purposes)
                Texture2D pixelTexture = new(GraphicsDevice, 1, 1);
                pixelTexture.SetData(new[] { Color.White });
                AssetManager._textures.Add("Pixel", pixelTexture);


                // InfoBackground
                AssetManager.LoadAsset<Texture2D>("Overworld/BackgroundMaps/InfoBG/InfoBG");

                // Fonts
                AssetManager.LoadAssetsFromFile<SpriteFont>("Fonts");

                /// General UI
                // Buttons
                AssetManager.LoadAssetsFromFile<Texture2D>("UI/Buttons");

                /// Towers
                AssetManager.LoadAssetsFromFile<Texture2D>("Towers");
                // Projectiles
                AssetManager.LoadAssetsFromFile<Texture2D>("Towers/Projectiles");
                
                /// Enemies
                AssetManager.LoadAssetsFromFile<Texture2D>("Enemies");
                // Enemy UI
                AssetManager.LoadAssetsFromFile<Texture2D>("UI/Actors/Enemies");
            }
            catch (DirectoryNotFoundException e)
            {
                Debug.WriteLine(e);
                Exit();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Exit();
            }

            /// JSON data loading
            // Load Towers
            TowerHolder.LoadTowers();
            // Load Enemies
            EnemyHolder.LoadEnemies();


            // Keep last, template code
            LateInitialize();
        }

        

        // Late Initialize (Anything that requires textures)
        void LateInitialize()
        {
            _thisGame = new TowerDefenceGame();
        }

        protected override void Update(GameTime gameTime)
        {
            // Exit input logic
            // TODO: Implement fucky wuckies uhhh quit button, that's what it's called
            if (KeyboardController.GetKeyPressed(Keys.Escape))
                Exit();


            // Game update
            _thisGame.Update(gameTime);


            // Keep last
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear Screen with default colour
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Render Game
            _spriteBatch.Begin();
            _thisGame.Render(ref _spriteBatch, Window);
            _spriteBatch.End();

            // Keep last
            base.Draw(gameTime);
        }
    }
}