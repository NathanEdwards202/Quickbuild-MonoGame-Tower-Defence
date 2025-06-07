using Misc;

namespace Actors.Characters.Towers
{
    internal class TowerStats : ActorStats
    {
        public TowerStats(TowerStatsTemplate template)
            : base(template._name, AssetManager.GetTexture(template._textureName), template._rotationSpeed)
        {
            _canMove = template._canMove;
            _movementSpeed = template._movementSpeed;
            _range = template._range;
            _damage = template._damage;
            _attackSpeed = template._attackSpeed;

            _projectileSpeed = template._projectileSpeed;
            _projectilePierce = template._projectilePierce;
            _projectileLife = template._projectileLife;
            _projectileTextureName = template._projectileTextureName;
        }

        public bool _canMove { get; private set; }
        public float _movementSpeed { get; private set; }
        public float _range { get; private set; }
        public float _damage { get; private set; }
        public float _attackSpeed { get; private set; }


        public float _projectileSpeed { get; private set; }
        public float _projectilePierce { get; private set; }
        public float _projectileLife { get; private set; }
        public string _projectileTextureName { get; private set; }
    }
}
