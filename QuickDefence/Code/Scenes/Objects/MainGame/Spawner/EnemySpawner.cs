using Actors.Enemies;
using Microsoft.Xna.Framework;
using Misc;
using Newtonsoft.Json.Linq;
using Scenes.Objects.UI.MainGame.SideBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Scenes.Objects.MainGame.Spawner
{
    internal class EnemySpawner
    {

        public EnemySpawner()
        {
            _wave = 1;
            _spawnType = 1;
            _spawning = false;
            _roundTimer = 0;
            _enemyQueue = new Queue<EnemySpawn>();

            SetupDelegates();

            if (this.GetType() == typeof(EnemySpawner)) Debug.WriteLine("Enemy Spawner initialized.");
        }

        ~EnemySpawner()
        {
            RemoveDelegates();
        }

        void SetupDelegates()
        {
            NextRoundButton.buttonClicked += StartRound;
        }

        void RemoveDelegates()
        {
            NextRoundButton.buttonClicked -= StartRound;
        }

        // Static as there will only ever be one of these things in the first place
        // This entire class could be static lol
        public static event EventHandler onWaveStarted;
        public static event EventHandler onWaveEnded;

        public int _maxWave;
        public int _wave { get; private set; }
        readonly int _spawnType;
        public bool _spawning {get; private set; }
        double _roundTimer;

        public Queue<EnemySpawn> _enemyQueue { get; private set; }

        // Thanks again GPT
        // The template for loading a level is reused here
        public void StartRound(object sender, EventArgs e)
        {
            string filePath = LevelLoaderManager.levelLoadingPath + "SpawnerData/SpawnType" + _spawnType.ToString() + ".json";

            // Get Data
            string jsonData = System.IO.File.ReadAllText(filePath);

            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(jsonData);

            List<EnemySpawn> sorter = new();

            // Acquire this round
            try
            {
                // Get all the wave numbers and determine the max wave
                _maxWave = jsonObject["rounds"].Children<JProperty>()
                    .Select(p => int.Parse(p.Name))
                    .Max();

                JObject thisRound = (JObject)jsonObject["rounds"][_wave.ToString()] ?? throw new Exception("Reached the final round already, implement game win smh");

                foreach (var enemySpawningSequence in thisRound)
                {
                    JObject spawningSequenceData = (JObject)enemySpawningSequence.Value;

                    for (int i = 0; i < spawningSequenceData["num"].ToObject<int>(); i++)
                    {
                        sorter.Add(
                            new EnemySpawn(
                                spawningSequenceData["name"].ToString(),
                                (spawningSequenceData["start"].ToObject<float>() + spawningSequenceData["delay"].ToObject<float>() * i) / 1000f
                                )
                            );
                    }
                }

                _enemyQueue = new Queue<EnemySpawn>(sorter.OrderBy(e => e._time).ToList());


                _roundTimer = 0;
                _spawning = true;
                onWaveStarted?.Invoke(this, EventArgs.Empty);
                _wave++;
            }
            catch (Exception ex) // This existed to stop it crashing after the final wave... Guess I forgot to remove it. Should never call
            {
                Debug.WriteLine($"{ex}");
                _wave = -1;
                onWaveStarted?.Invoke(this, EventArgs.Empty);
                return;
            }
        }

        // Called ""recursively"" to spawn all enemies
        public EnemyStatsTemplate? Update(GameTime gameTime, bool firstCallThisFrame)
        {
            if(firstCallThisFrame) _roundTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (!(_enemyQueue.Count == 0) && _roundTimer > _enemyQueue.Peek()._time) // Check if enemy ready to spawn
            {
                EnemyStatsTemplate returnVal = EnemyHolder._enemies[_enemyQueue.Dequeue()._name];

                if (_enemyQueue.Count == 0) // Spawning completed after this one
                {
                    _spawning = false;
                    onWaveEnded?.Invoke(this, EventArgs.Empty);
                    Debug.WriteLine("Round spawning finished");
                }

                return returnVal; // Return the enemy to spawn's template
            }
            else
            {
                return null;
            }
        }

        public static Enemy SpawnEnemy(Paths.Path path, EnemyStatsTemplate template)
        {
            return new Enemy(path, new(template));
        }
    }

    public struct EnemySpawn
    {
        public EnemySpawn(string name, float time)
        {
            _name = name;
            _time = time;
        }

        public string _name { get; private set; }
        public float _time;
    }
}
