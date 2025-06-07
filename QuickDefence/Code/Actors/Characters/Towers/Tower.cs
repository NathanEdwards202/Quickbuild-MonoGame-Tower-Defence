using Actors.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using QuickDefence.Code.Actors;
using Scenes.Objects.MainGame.PlacementBlockers;
using System;
using System.Linq;

#nullable enable

namespace Actors.Characters.Towers
{
    internal class Tower : Actor
    {
        public Tower(TowerStats stats, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default)
            : base(stats._name, stats, position, layer, rotation, stats._texture.Bounds.Size.ToVector2())
        {
            _stats = stats;
            _texture = _stats._texture;

            _shootTimer = 0;
        }

        public void Instantiate()
        {
            _thisBlocker = new(
                position: _position,
                size: _size
                );
        }

        PlacementBlocker? _thisBlocker;

        Enemy? _target;

        public new TowerStats _stats { get; protected set; }
        public float _shootTimer { get; protected set; }

        public override void Update(GameTime gameTime)
        {
            float distance = UpdateTarget();
            DoRotation(gameTime, GetTargetRotation(gameTime, distance) ?? _rotation);
            DoAttack(gameTime);

            base.Update(gameTime);
        }


        protected virtual float UpdateTarget()
        {
            // Target is the Enemy within range that has moved the furthest distance along the track
            Enemy? targetEnemy = ObjectManager._objects
                .OfType<Enemy>()
                .Where(o => ObjectManager.GetDistanceFromOrigin(this, o) <= _stats._range)
                .OrderByDescending(e => e._distance)
                .FirstOrDefault();

            if (targetEnemy != null)
            {
                _target = targetEnemy;
                return ObjectManager.GetDistanceFromOrigin(this, _target);
            }
            else
            {
                _target = null;
                return 0;
            }
        }

        protected virtual float? GetTargetRotation(GameTime gameTime, float distance)
        {
            if (_target == null)
            {
                return null;
            }
                

            // Calculate the vector pointing from 'position' to '_target'
            Vector2 dir = PredictTargetMovement(gameTime, distance) - _position;

            // Calculate the angle in radians from the positive X-axis to the direction vector
            float angle = MathF.Atan2(dir.Y, dir.X);

            // Adjust the angle to match sprites which I made facing upwards rather than to the right
            // I HATE RADIAAAAANS
            float targetRotation = angle + MathF.PI / 2;

            // Normalize rotation to be within 0 to 2 * MathF.PI
            if (targetRotation < 0)
            {
                targetRotation += 2 * MathF.PI;
            }
            else if (targetRotation >= 2 * MathF.PI)
            {
                targetRotation -= 2 * MathF.PI;
            }

            return targetRotation;
        }

        protected virtual Vector2 PredictTargetMovement(GameTime gameTime, float distance)
        {
            //Vector2 diff = _target!._target - _target._position + _target._size / 2;

            Vector2 diff = _target!._target - _target._path._points[_target._pointReached]._position;
            diff.Normalize();
            Vector2 distanceMoved = (float)_target._stats._speed * (float)gameTime.ElapsedGameTime.TotalSeconds * diff;

            return _target._position + distanceMoved + diff * (distance / _stats._projectileSpeed); // This is a bad approximation
                                                                                                    // Will be off more the further away it is
                                                                                                    // Will be off more the slower the projectile
        }


        protected virtual void DoAttack(GameTime gameTime)
        {
            if (_target == null) // Yeah, this might be kind of cruel to slow-shooting towers
            {
                _shootTimer = 0;
                return;
            }

            _shootTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_shootTimer >= _stats._attackSpeed)
            {
                Shoot();
                _shootTimer -= _stats._attackSpeed;
            }
        }

        void Shoot()
        {
            Projectile newProjectile = new(
                this
                );

            ObjectManager.onGameObjectCreated?.Invoke(null, new ObjectManager.OnGameObjectCreatedEventArgs { obj = newProjectile });
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
#if DEBUG
            _thisBlocker?.Render(ref sb, window);
#endif
            base.Render(ref sb, window);
        }

        public override void RenderRelative(ref SpriteBatch sb, GameWindow window, Vector2 relativeTo)
        {
            base.RenderRelative(ref sb, window, relativeTo);
        }
    }
}
