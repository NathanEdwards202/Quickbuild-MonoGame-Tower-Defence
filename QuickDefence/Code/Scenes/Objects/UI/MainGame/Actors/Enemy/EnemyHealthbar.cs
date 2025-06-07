using Actors.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;

namespace Scenes.Objects.UI.MainGame.Actors.Enemy
{
    internal class EnemyHealthbar : UIElement
    {
        public EnemyHealthbar(EnemyStats eStats, Texture2D texture = null, Vector2 position = default, int layer = 1, float rotation = 0, Vector2 size = default)
            : base(texture, position, layer, rotation, size)
        {
            _filling = new(
                    texture: AssetManager._textures["EnemyHealthBarFill"], // Forgot an "ed" on this texture name, huh
                    position: new(BAR_POSITION_X, BAR_POSITION_Y),
                    size: new(BAR_WIDTH, BAR_HEIGHT)
                );

            _background = new(
                    texture: AssetManager._textures["EnemyHealthBarBackground"],
                    position: new(BAR_POSITION_X, BAR_POSITION_Y),
                    size: new(BAR_WIDTH, BAR_HEIGHT)
                );

            SetupDelegates(eStats);

            _percent = 1;
        }

        // Remove delegates should not be necessary,
        // The caller calls onDamageTakenSendPercentHP = null; via it's finalizer
        void SetupDelegates(EnemyStats eStats)
        {
            eStats.onDamageTakenSendPercentHP += OnEnemyHitUpdatePercent;
        }

        const int BAR_POSITION_X = 16;
        const int BAR_POSITION_Y = -16;
        const int BAR_WIDTH = 24;
        const int BAR_HEIGHT = 6;

        readonly EnemyHealthbarPart _filling;
        readonly EnemyHealthbarPart _background;

        private float _percent;

        void OnEnemyHitUpdatePercent(object sender, EnemyStats.OnDamageTakenEventArgs e)
        {
            _percent = e.percentHP;
        }

        public override void RenderRelative(ref SpriteBatch sb, GameWindow window, Vector2 relativeTo)
        {
            Vector2 origin = new(_size.X / 2f, _size.Y / 2f);

            sb.Draw(
                texture: _background._texture,
                destinationRectangle: new Rectangle(
                        (int)relativeTo.X + (int)_background._position.X + (int)origin.X,
                        (int)relativeTo.Y + (int)_background._position.Y + (int)origin.Y,
                        (int)_background._size.X,
                        (int)_background._size.Y
                        ),
                sourceRectangle: null,
                color: Color.White,
                rotation: _rotation,
                origin: origin,
                effects: SpriteEffects.None,
                layerDepth: 0
                );

            sb.Draw(
                texture: _filling._texture,
                destinationRectangle: new Rectangle(
                        (int)relativeTo.X + (int)_filling._position.X + (int)origin.X,
                        (int)relativeTo.Y + (int)_filling._position.Y + (int)origin.Y,
                        (int)(_filling._size.X * _percent),
                        (int)_filling._size.Y
                        ),
                sourceRectangle: null,
                color: Color.White,
                rotation: _rotation,
                origin: origin,
                effects: SpriteEffects.None,
                layerDepth: 0
                );
        }
    }

    // Huh......
    // Yeah, there is literally no reason for this to exist while UIElement isn't abstract
    internal class EnemyHealthbarPart: UIElement
    {
        public EnemyHealthbarPart(Texture2D texture, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default)
            : base(texture, position, layer, rotation, size)
        {
        }
    }
}
