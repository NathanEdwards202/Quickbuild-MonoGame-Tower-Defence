using Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using Newtonsoft.Json.Linq;
using QuickDefence.Code.Misc;
using Scenes.Objects;
using Scenes.Objects.MainGame.Background;
using Scenes.Objects.MainGame.Paths;
using Scenes.Objects.MainGame.Spawner;
using Scenes.Objects.UI.MainGame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scenes.Objects.MainGame.PlacementBlockers;
using Actors.Enemies;

namespace Scenes.SceneTypes
{
    internal class MainGameScene : Scene
    {
        public MainGameScene(string sceneJSONDataFilePath) : base()
        {
            LoadSceneFromJSON(sceneJSONDataFilePath);

            Player.OnGameStartSetup();
            AddToScene(MainGameUICreator.GenerateSceneUIElements());

            _enemySpawner = new EnemySpawner();

            ObjectManager._dirtyLayer = true;

            if (this.GetType() == typeof(MainGameScene)) Debug.WriteLine($"{_sceneName} initialized.");
        }

        public string _sceneName { get; private set; }


        const int BG_WIDTH = 1420;
        const int BG_HEIGHT = 1080;
        BackgroundImage _backgroundTexture;


        public List<Path> _paths { get; private set; }

        public EnemySpawner _enemySpawner { get; private set; }

        bool _gameLost = false;
        bool _gameWon = false;
        public static EventHandler _onWin, _onLoss; // Fucking EWW
                                                    // I mean... It works, but I do not like how I handled events in this program at all


        // Cheers GPT, gave me the template to get JSON stuff working
        // I literally had no idea before
        void LoadSceneFromJSON(string sceneJSONData)
        {
            // Get full path
            string filePath = LevelLoaderManager.levelLoadingPath + sceneJSONData;

            // Get Data
            string jsonData = System.IO.File.ReadAllText(filePath);

            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(jsonData);


            // Scene name
            _sceneName = jsonObject["sceneName"].ToString();

            // Paths
            _paths = new List<Path>();

            JObject paths = (JObject)jsonObject["paths"];
            foreach(var path in paths)
            {
                JObject pathData = (JObject)path.Value;

                List<PathPoint> points = new();

                foreach (var point in pathData)
                {
                    JObject pointData = (JObject)point.Value;

                    // Extract position
                    JObject position = (JObject)pointData["position"];
                    float x = position["x"].ToObject<float>();
                    float y = position["y"].ToObject<float>();

                    // Extract pointID
                    int pointID = pointData["pointID"].ToObject<int>();

                    // Step 6: Create a new PathPoint and add it to the _path list
                    points.Add(new PathPoint((ushort)pointID, new Vector2(x, y)));
                }

                _paths.Add(new Path(points));
            }

            // The rest of this is non-gpt

            // Static Objects (I just didn't put any on the map lol)

            // Generate path placement blockers
            List<PlacementBlocker> pathBlockers = PlacementBlocker.GeneratePlacementBlockersFromPaths(_paths);
            foreach (var pathBlocker in pathBlockers)
            {
                ObjectManager._objects.Add(pathBlocker);
            }

            // BGTexture name
            AssetManager.OnLoadLevel(jsonObject["BGTextureName"].ToString());
            Texture2D bgTexture = AssetManager.GetTexture(AssetManager._currentMapTexture);

            // Create the map background
            _backgroundTexture = new(texture: bgTexture, size: new(BG_WIDTH, BG_HEIGHT));
            AddToScene(_backgroundTexture);
        }

        public override void Update(GameTime gameTime)
        {
            base.GetInputs(gameTime); // Of course

            if (_gameWon || _gameLost) return; // I uh... Yeah, this is bad... But I didn't wanna properly make win / loss screens

            base.Update(gameTime);

            // The actual update logic
            DoEnemySpawning(gameTime);
            CheckWin();
            DoObjectRemoval();
        }

        void CheckWin()
        {
            if (Player._lives <= 0)
            {
                _gameLost = true;
                _onLoss?.Invoke(null, null);
            }

            // Final wave, no enemies left to spawn, no enemies alive, player not dead
            if (_enemySpawner._wave == _enemySpawner._maxWave
                && _enemySpawner._enemyQueue.Count == 0
                && ObjectManager._objects.OfType<Enemy>().Count() == 0 // Today I learnt that .Any() is faster than .Count() == 0. Cheers Visual Studio messages
                && Player._lives > 0)
            {
                _gameWon = true;
                _onWin?.Invoke(null, null);
            }
        }

        // spawn them fuckers, handles multiple enemies spawning in a given frame
        void DoEnemySpawning(GameTime gameTime)
        {
            // Spawn enemies
            EnemyStatsTemplate? template = _enemySpawner.Update(gameTime, firstCallThisFrame: true);

            while (template != null)
            {
                AddToScene(EnemySpawner.SpawnEnemy(_paths[RandomHandler._random.Next(minValue: 0, maxValue: _paths.Count)],
                    (EnemyStatsTemplate)template)); // can cast because this will never be null

                template = _enemySpawner.Update(gameTime, firstCallThisFrame: false);
            }
        }

        // Yes this should be handled in the ObjectManager
        // No. It is not.
        static void DoObjectRemoval()
        {
            List<GameObject> removalObjects = ObjectManager._objects.Where(o => o._forDeletion == true).ToList();

            foreach(GameObject obj in removalObjects)
            {
                RemoveFromScene(obj);
            }

            if (removalObjects.Count > 100) // I don't think I've ever managed to have this condition get called lol
            {
                GC.Collect();
            }
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            _backgroundTexture.Render(ref sb, window);
            base.Render(ref sb, window);
        }
    }
}
