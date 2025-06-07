using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuickDefence.Code.Misc;
using Scenes.Objects.UI.Text;
using System;

namespace Scenes.Objects.UI.MainGame.Actors.Enemy
{
    internal class EnemyTakenDamageText : TextDisplay
    {
        public EnemyTakenDamageText(SpriteFont font, string defaultTextValue, Vector2 position)
            : base(font, defaultTextValue, position, -2, 0, new(SIZE_X, SIZE_Y))
        {
            _size = new(
                _size.X * defaultTextValue.Length,
                _size.Y
                );

            // RandomHandler gets used four times total.... ALL HERE YIPPEEEE
            _xOffset = RandomHandler.GenerateRandomFloat(MIN_X_OFFSET, MAX_X_OFFSET);
            _yOffset = RandomHandler.GenerateRandomFloat(MIN_Y_OFFSET, MAX_Y_OFFSET);
            _position = new(
                _position.X + _xOffset,
                _position.Y + _yOffset
                );
            _dir = new(_xOffset, -_yOffset); // negative to flip Y (I don't know why this is necessary but it is)
            _dir.Normalize();
            _speed = RandomHandler.GenerateRandomFloat(MIN_SPEED, MAX_SPEED);
            _duration = RandomHandler.GenerateRandomFloat(MIN_DURATION, MAX_DURATION);
            _currentDuration = 0;
        }

        // Love me some consts
        const int SIZE_X = 12;
        const int SIZE_Y = 18;
        const int MIN_X_OFFSET = -5;
        const int MAX_X_OFFSET = 5;
        const int MIN_Y_OFFSET = 16;
        const int MAX_Y_OFFSET = 18;
        const float MIN_SPEED = 40f;
        const float MAX_SPEED = 60f;
        const float SPEED_MULTI = 500f;
        const float deceleration = 40f / 3.6f;
        const float MIN_DURATION = 2.4f;
        const float MAX_DURATION = 3.6f;

        readonly float _xOffset;
        readonly float _yOffset;
        Vector2 _dir;
        float _speed;
        readonly float _duration;
        float _currentDuration;

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _currentDuration += deltaTime;
            if (_currentDuration > _duration)
            {
                _forDeletion = true;
                return;
            }

            Vector2 distanceMoved = _speed * deltaTime * _dir * SPEED_MULTI;
            // Proper acceleration-delta-time-based movement
            _position += distanceMoved / 2;
            _speed -= deceleration * deltaTime;
            distanceMoved = _speed * deltaTime * _dir * SPEED_MULTI;
            _position -= distanceMoved / 2;

            _alpha = 1f - (_currentDuration / _duration);
        }

        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            Vector2 textSize = _font.MeasureString(_textValue);
            Vector2 scale = new(_size.X / textSize.X, _size.Y / textSize.Y);
            Vector2 origin = new(textSize.X / 2f, textSize.Y / 2f);

            sb.DrawString(
                spriteFont: _font,
                text: _textValue,
                position: new(
                    (int)_position.X + (int)(origin.X * scale.X),
                    (int)_position.Y + (int)(origin.Y * scale.Y)
                    ),
                color: Color.Red * MathF.Pow(_alpha, 2f), // Pow it to have it fade out quickly, then slowly
                rotation: 0f,
                origin: origin,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }
    }
}