using Actors.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using Scenes.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Actors.Characters.Towers
{
    internal class Projectile : GameObject
    {
        public Projectile(Tower spawner) 
            : base(spawner._position + spawner._size / 2, 1, spawner._rotation, 
                  AssetManager._textures[spawner._stats._projectileTextureName].Bounds.Size.ToVector2(),
                  1)
        {
            _position -= _size / 2;
            _damage = spawner._stats._damage;
            _texture = AssetManager.GetTexture(spawner._stats._projectileTextureName);
            _speed = spawner._stats._projectileSpeed;
            _pierce = spawner._stats._projectilePierce;
            _life = spawner._stats._projectileLife;
            _timeSpent = 0;

            _alreadyHit = new();
        }

        public float _damage { get; private set; }
        public float _speed { get; private set; }
        public float _pierce { get; private set; }
        public float _life { get; private set; }
        public float _timeSpent { get; private set; }

        List<Enemy> _alreadyHit; // Prevent machine-gun hitting enemies each frame. Each projectile can only hit an enemy once
                                 // I cannot tell why Visual Studio wants me to make this readonly

        public override void Update(GameTime gameTime)
        {
            _timeSpent += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timeSpent >= _life)
            {
                _forDeletion = true;
                return;
            }

            DoMovement(gameTime);
            DoDamaging();
        }

        void DoMovement(GameTime gameTime)
        {
            Vector2 dir = GetNormalizedVectorFromAngleWithNinetyDegreesCorrection(_rotation);

            _position += dir * _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        // Do damage to all things hit by this projectile until out of enemies or pierce
        void DoDamaging()
        {
            List<Enemy> enemiesHit = ObjectManager._objects
                .OfType<Enemy>()
                .Where(o => o._forDeletion == false && ObjectManager.GetObjectOverlap(this, o))
                .OrderBy(o => ObjectManager.GetDistanceFromOrigin(this, o))
                .ToList();

            for(int i = 0; i < enemiesHit.Count; i++)
            {
                if (_alreadyHit.Contains(enemiesHit[i])) continue;

                enemiesHit[i]._stats.TakeDamage(_damage);
                _alreadyHit.Add(enemiesHit[i]);
                _pierce--;
                if (_pierce <= 0)
                {
                    _forDeletion = true;
                    return; // Yippee I remembered to tell it to stop when deleting
                }
            }
        }

        public static Vector2 GetNormalizedVectorFromAngleWithNinetyDegreesCorrection(float angle)
        {
            angle -= MathF.PI / 2;

            // Return the normalized vector (X, Y) based on the angle in radians
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            base.Render(ref sb, window);
        }

        public override void RenderRelative(ref SpriteBatch sb, GameWindow window, Vector2 relativeTo)
        {
            base.RenderRelative(ref sb, window, relativeTo);
        }
    }
}
