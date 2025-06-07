using Actors.Characters.Towers;
using Controllers.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using Scenes.Objects.MainGame.PlacementBlockers;
using Scenes.Objects.UI.Interactable.Buttons;
using System;
using System.Linq;
using System.Reflection;

namespace Scenes.Objects.UI.MainGame.Actors.Characters
{
    internal class TowerPreview : UIElement
    {
        public TowerPreview(GameObject activatedFrom, Tower tower, Vector2 position = default, int layer = 1, float rotation = 0, Vector2 size = default, float alpha = 0.5f) 
            : base(tower._texture, position, layer, rotation, size, alpha)
        {
            _objectActivatedFrom = activatedFrom;
            _thisTower = tower;
            _invalidPlacementSprite = AssetManager.GetTexture("Pixel");
            _rangeDisplaySprite = AssetManager.CreateCircleTexture((int)_thisTower._stats._range); // Memory go brrrrrrrrrrrrrrrrrrrr
                                                                                                   // This should probably be a smaller circle stretched rather than full-on range
                                                                                                   // O(n^2) and all
            _validPlacement = false;

            _placementButton = new(
                linkedTo: this,
                defaultState: ButtonState.Deactivated
                );

            _destroyOnOutOfBounds = false;

            ObjectManager.onTowerPreviewCreated?.Invoke(null, null);

            SetupDelegates();
        }

        ~TowerPreview()
        {
            RemoveDelegates();
        }

        void SetupDelegates()
        {
            _placementButton.buttonClicked += SpawnTower;
        }

        void RemoveDelegates()
        {
            _placementButton.buttonClicked -= SpawnTower;
            _placementButton.Dispose(); // Force dispose, clean that shit up
        }

        GameObject _objectActivatedFrom; // WHY DID I MAKE YOU??????????????????
        public Tower _thisTower { get; private set; }
        readonly Texture2D _invalidPlacementSprite;
        readonly Texture2D _rangeDisplaySprite;
        bool _validPlacement;
        readonly Button _placementButton;
        bool _destroyOnOutOfBounds;

        const int TOP_LEFT_X = 0;
        const int TOP_LEFT_Y = 0;
        const int BOTTOM_RIGHT_X = 1920 - 500;
        const int BOTTOM_RIGHT_Y = 1080;

        public override void Update(GameTime gameTime)
        {
            _position = MouseController.GetPosition()
                - _size / 2;
            _placementButton.Update(gameTime);

            // Check if mouse is not over the map
            if (!MouseController.GetMouseOverlap(new Rectangle(
                TOP_LEFT_X + (int)(_size.X / 2), // THIS PLUS SHOULD NOT BE HERE I WANT MOUSE POSITION NOT BOTTOM-RIGHT OF TOWER POSITION AAAA
                TOP_LEFT_Y + (int)(_size.Y / 2), // THIS PLUS SHOULD ALSO NOT BE HERE AAAA
                BOTTOM_RIGHT_X - (int)_size.X, // ??? WHY DID I NOT DIVIDE THIS ONE BY 2 ???
                BOTTOM_RIGHT_Y - (int)_size.Y)
                )) 
            {
                _validPlacement = false;
                _placementButton.DeactivateButton();

                if (_destroyOnOutOfBounds) Delete();
                return;
            }

            // If the mouse has been over the map, then
            // Destroy this if the mouse leaves the map
            // Forgor to return
            if (!_destroyOnOutOfBounds) _destroyOnOutOfBounds = true;

            // Check if mouse is over a placement blocker... Or a GameObject which has a placement blocker
            foreach (GameObject obj in ObjectManager._objects.Where(o =>
                o.GetType() == typeof(PlacementBlocker) ||
                o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Any(f => f.FieldType == typeof(PlacementBlocker) && f.GetValue(o) is PlacementBlocker) ||
                o.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Any(p => p.PropertyType == typeof(PlacementBlocker) && p.GetValue(o) is PlacementBlocker)
            ))
            {
                if (ObjectManager.GetObjectOverlap(this, obj))
                {
                    _validPlacement = false;
                    _placementButton.DeactivateButton();
                    return;
                }
            }

            // If all that is fine, then you can place it
            _placementButton.ActivateButton();
            _validPlacement = true;
        }

        void SpawnTower(object sender, EventArgs e)
        {
            Tower t =
                new(
                    stats: _thisTower._stats,
                    position: _position,
                    layer: 1,
                    rotation: 0,
                    size: _thisTower._size
                    );
            t.Instantiate();
            ObjectManager.onGameObjectCreated?.Invoke(null, new ObjectManager.OnGameObjectCreatedEventArgs { obj = t });

            Delete();
        }

        void Delete()
        {
            // Honestly, there's probably a frame-perfect crash this can cause
            // I'm just not 100% sure what it is right now :D
            ObjectManager.onTowerPreviewDestroyed?.Invoke(null, null);
            _forDeletion = true;
        }



        public override void Render(ref SpriteBatch sb, GameWindow window)
        {
            Vector2 origin = new(_size.X / 2f, _size.Y / 2f);
            Vector2 centerPosition = _position + origin;
            float rangeCircle = 2 * _thisTower._stats._range;

            // Range preview
            sb.Draw(
                texture: _rangeDisplaySprite,
                destinationRectangle: new Rectangle(
                    (int)(centerPosition.X - rangeCircle / 2),
                    (int)(centerPosition.Y - rangeCircle / 2),
                    (int)rangeCircle,
                    (int)rangeCircle
                ),
                sourceRectangle: null,
                color: (_validPlacement ? Color.White : Color.Red) * _alpha * 0.5f,
                rotation: 0,
                origin: Vector2.Zero,
                effects: SpriteEffects.None,
                layerDepth: 0
            );

            // Show the hitbox if you cannot place it here
            if (!_validPlacement)
            {
                sb.Draw(
                    texture: _invalidPlacementSprite,
                    destinationRectangle: new Rectangle(
                        (int)_position.X,
                        (int)_position.Y,
                        (int)_size.X,
                        (int)_size.Y
                    ),
                    sourceRectangle: null,
                    color: Color.Red * _alpha,
                    rotation: 0,
                    origin: Vector2.Zero,
                    effects: SpriteEffects.None,
                    layerDepth: 0
                );
            }

            // Preview of the tower itself
            sb.Draw(
                texture: _thisTower._texture,
                destinationRectangle: new Rectangle(
                        (int)_position.X + (int)origin.X,
                        (int)_position.Y + (int)origin.Y,
                        (int)_size.X,
                        (int)_size.Y
                        ),
                sourceRectangle: null,
                color: Color.White * _alpha,
                rotation: _rotation,
                origin: origin,
                effects: SpriteEffects.None,
                layerDepth: 0
                );
        }
    }
}