using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using QuickDefence.Code.Actors;
using Scenes.Objects.MainGame.Paths;
using Scenes.Objects.UI.MainGame.Actors.Enemy;
using System;
using System.Diagnostics;

namespace Actors.Enemies
{
    internal class Enemy : Actor
    {
        public Enemy(Path path, EnemyStats stats, int layer = 0, float rotation = 0)
            : base(stats._name, stats, path._points[0]._position, layer, rotation, stats._texture.Bounds.Size.ToVector2())
        {
            // path to follow
            _path = path;
            _pointReached = 0;
            _distance = 0;
            // Initial point to move to
            UpdateTarget();
            UpdateTargetRotation();

            // stats
            _stats = stats;
            _texture = stats._texture;

            // Healthbar
            _healthBar = new(_stats);

            // Literally just because I can in this final build
            _layer = _name switch
            {
                "ScurryBug" => 1,
                "Spooky" => 2,
                "OrangeBloon" => 3,
                "Crab" => 4,
                "Dragon" => 5,
                _ => 0
            };


            SetupDelegates();

            if (this.GetType() == typeof(Enemy)) Debug.WriteLine($"Enemy {_name} spawned");
        }

        ~Enemy()
        {
            RemoveDelegates();
        }

        void SetupDelegates()
        {
            _stats.onDamageTakenSendPercentHP += OnHitCreateDamageText;
            _stats.onOutOfHP += OnDie;
        }

        void RemoveDelegates()
        {
            _stats.onDamageTakenSendPercentHP -= OnHitCreateDamageText;
            _stats.onOutOfHP -= OnDie;
        }

        public Path _path {get; private set;}
        public Vector2 _target { get; private set; }
        float _targetRotation;
        public int _pointReached {get; private set;}
        public float _distance { get; private set; }

        public new EnemyStats _stats { get; private set; }

        readonly EnemyHealthbar _healthBar;

        public override void Update(GameTime gameTime)
        {
            DoMovement(gameTime);
            DoRotation(gameTime, _targetRotation);
        }

        void DoMovement(GameTime gameTime)
        {
            // Find distance to path
            Vector2 difference = _target - _position;
            if (difference.Length() < 5f)
            {
                // Update point to move to
                _pointReached++;

                // When reaching the end, damage the player, delete the self
                // Why I didn't use Path.CheckEndOfTrack I will never know
                if (_pointReached == _path._points.Count - 1)
                {
                    Player.OnEnemyReachedEnd(_stats._damage);
                    _forDeletion = true;
                    return;
                }

                // Such intuitive reading, many wow
                if (_path._points[_pointReached - 1]._position.X == _path._points[_pointReached]._position.X)
                {
                    _position = new(_path._points[_pointReached]._position.X, _position.Y);
                }
                else if(_path._points[_pointReached - 1]._position.Y == _path._points[_pointReached]._position.Y)
                {
                    _position = new(_position.X, _path._points[_pointReached]._position.Y);
                }

                // Move but to new path
                UpdateTarget();
                DoMovement(gameTime);
            }

            // Get direction
            difference.Normalize();
            // Find distance to move
            Vector2 distanceMoved = (float)_stats._speed * (float)gameTime.ElapsedGameTime.TotalSeconds * difference;
            _distance += MathF.Sqrt(distanceMoved.LengthSquared());

            // Move
            _position += distanceMoved;
        }

        void UpdateTarget()
        {
            _target = _path._points[_pointReached + 1]._position;
            UpdateTargetRotation();
        }

        void UpdateTargetRotation()
        {
            Vector2 diff = _target - _position;
            diff.Normalize();
            Vector2 rotationCorrecton = new(-diff.Y, diff.X); // I don't get radians
                                                              // Has something to do with me making all my sprites facing up instead of facing right, I think
                                                              // I really don't know

            _targetRotation = GetRadiansFromNormalizedVector(rotationCorrecton);
        }

        void OnHitCreateDamageText(object sender, EnemyStats.OnDamageTakenEventArgs e)
        {
            EnemyTakenDamageText textObj = new(
                    font: AssetManager.GetFont("DefaultFontArial"),
                    defaultTextValue: ((int)e.damage).ToString(),
                    position: _position
                );

            ObjectManager.onGameObjectCreated?.Invoke(null, new ObjectManager.OnGameObjectCreatedEventArgs { obj = textObj });
        }

        void OnDie(object sender, EventArgs e)
        {
            _forDeletion = true;
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            base.Render(ref sb, window);

            if(_stats._currentHp != _stats._maxHP) _healthBar.RenderRelative(ref sb, window, _position);
        }
    }
}