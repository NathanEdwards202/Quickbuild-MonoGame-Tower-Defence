using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Misc
{
    internal static class EnemyHolder
    {
        public static Dictionary<string, EnemyStatsTemplate> _enemies = new();

        public static void LoadEnemies()
        {
            string filePath = LevelLoaderManager.levelLoadingPath + "EnemyData/Enemies.json";

            // Get Data
            string jsonData = System.IO.File.ReadAllText(filePath);

            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(jsonData);

            // Convert enemies to templates
            JObject enemies = (JObject)jsonObject["enemies"];
            foreach(var enemy in enemies)
            {
                JObject enemyData = (JObject)enemy.Value;

                EnemyStatsTemplate e = new(
                    enemy.Key,
                    enemyData["health"].ToObject<float>(),
                    enemyData["speed"].ToObject<float>(),
                    enemyData["rotationSpeed"].ToObject<float>(),
                    enemyData["damage"].ToObject<float>(),
                    enemyData["texture"].ToObject<string>()
                    );

                _enemies[enemy.Key] = e;
            }

            Debug.WriteLine("Enemies Loaded");
        }
    }

    public struct EnemyStatsTemplate
    {
        public EnemyStatsTemplate(string name, float hp, float spd, float rotationSpeed, float dmg, string textureName)
        {
            _name = name;
            _hp = hp;
            _spd = spd;
            _rotationSpeed = rotationSpeed;
            _dmg = dmg;
            _textureName = textureName;

            Debug.WriteLine($"Template for enemy {_name} loaded");
        }

        public string _name { get; private set; }
        public float _hp { get; private set; }
        public float _spd { get; private set; }
        public float _rotationSpeed { get; private set; }
        public float _dmg { get; private set; }
        public string _textureName {  get; private set; }
    }
}
