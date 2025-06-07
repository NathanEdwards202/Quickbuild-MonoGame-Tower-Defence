using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace Misc
{
    internal static class TowerHolder
    {
        // Ooooo yeah, hold them towers
        public static Dictionary<string, TowerStatsTemplate> _towers = new();

        public static void LoadTowers()
        {
            string filePath = LevelLoaderManager.levelLoadingPath + "TowerData/Towers.json";

            // Get Data
            string jsonData = System.IO.File.ReadAllText(filePath);

            // Parse the JSON string into a JObject
            JObject jsonObject = JObject.Parse(jsonData);

            // Convert enemies to templates
            JObject towers = (JObject)jsonObject["towers"];
            foreach (var tower in towers)
            {
                JObject towerData = (JObject)tower.Value;

                TowerStatsTemplate e = new(
                    tower.Key,
                    towerData["canMove"].ToObject<bool>(),
                    towerData["movementSpeed"].ToObject<float>(),
                    towerData["range"].ToObject<float>(),
                    towerData["damage"].ToObject<float>(),
                    towerData["attackSpeed"].ToObject<float>(),
                    towerData["rotationSpeed"].ToObject<float>(),
                    towerData["texture"].ToString(),

                    towerData["projectileSpeed"].ToObject<float>(),
                    towerData["projectilePierce"].ToObject<float>(),
                    towerData["projectileLife"].ToObject<float>(),
                    towerData["projectileTexureName"].ToString()
                    );

                _towers[tower.Key] = e;
            }

            Debug.WriteLine("Towers Loaded");
        }

        // :)
        public static void CreateTowers()
        {

        }
    }

    public struct TowerStatsTemplate
    {
        public TowerStatsTemplate(string name, bool canMove, float movementSpeed, float range, float damage, float attackSpeed,
        float rotationSpeed, string textureName, float projectileSpeed, float projectilePierce, float projectileLife,
        string projectileTextureName)
        {
            _name = name;
            _canMove = canMove;
            _movementSpeed = movementSpeed;
            _range = range;
            _damage = damage;
            _attackSpeed = attackSpeed;
            _rotationSpeed = rotationSpeed;
            _textureName = textureName;

            _projectileSpeed = projectileSpeed;
            _projectilePierce = projectilePierce;
            _projectileLife = projectileLife;
            _projectileTextureName = projectileTextureName;
        }


        public string _name { get; private set; }
        public bool _canMove { get; private set; }
        public float _movementSpeed { get; private set; }
        public float _range { get; private set; }
        public float _damage { get; private set; }
        public float _attackSpeed { get; private set; }
        public float _rotationSpeed { get; private set; }
        public string _textureName { get; private set; }


        public float _projectileSpeed { get; private set; }
        public float _projectilePierce { get; private set; }
        public float _projectileLife { get; private set; }
        public string _projectileTextureName { get; private set; }
    }
}
