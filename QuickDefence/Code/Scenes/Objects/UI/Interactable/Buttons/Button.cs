using Controllers.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;


namespace Scenes.Objects.UI.Interactable.Buttons
{
    enum ButtonState
    {
        Interactable,
        Hovering,
        Held,
        Pressed,
        Deactivated
    }

    internal class Button : ClickableUIElement
    {
        // Beeeeeeeeg constructor parameters
        public Button(ButtonStateRenderingHolder interactableHolder, ButtonStateRenderingHolder? hoveringHolder = null,
            ButtonStateRenderingHolder? heldHolder = null, ButtonStateRenderingHolder? pressedHolder = null, ButtonStateRenderingHolder? deactivatedHolder = null,
            ButtonState defaultState = ButtonState.Interactable, Vector2 position = default, int layer = 0, float rotation = 0, Vector2 size = default)
            : base(null, position, layer, rotation, size)
        {
            _interactableStateRenderingHolder = interactableHolder;
            _hoveringStateRenderingHolder = hoveringHolder ?? _interactableStateRenderingHolder;
            _heldStateRenderingHolder = heldHolder ?? _interactableStateRenderingHolder;
            _pressedStateRenderingHolder = pressedHolder ?? _interactableStateRenderingHolder;
            _deactivatedStateRenderingHolder = deactivatedHolder ?? _interactableStateRenderingHolder;

            ChangeState(defaultState);

            if (this.GetType() == typeof(Button)) Debug.WriteLine("Button initialized.");
        }

        // Invisible and following object x
        // (Tower preview button)
        public Button(GameObject linkedTo, ButtonState defaultState = ButtonState.Interactable, int layer = 0, float rotation = 0)
            : base(null, linkedTo._position, layer, rotation, linkedTo._size)
        {
            _interactableStateRenderingHolder = new(null, 0);
            _hoveringStateRenderingHolder = _interactableStateRenderingHolder;
            _heldStateRenderingHolder = _interactableStateRenderingHolder;
            _pressedStateRenderingHolder = _interactableStateRenderingHolder;
            _deactivatedStateRenderingHolder = _interactableStateRenderingHolder;

            _linkedTo = linkedTo;

            ChangeState(defaultState);

            if (this.GetType() == typeof(Button)) Debug.WriteLine("Button initialized.");
        }

        ~Button()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            buttonClicked = null; // Prevent memory leaks when this thing no longer exists. This has to be called manually I think.
                                  // I'm pretty sure as long as someone is subscribed to this event the garbage collector won't pick this up
        }

        protected ButtonStateRenderingHolder _interactableStateRenderingHolder;
        protected ButtonStateRenderingHolder _hoveringStateRenderingHolder;
        protected ButtonStateRenderingHolder _heldStateRenderingHolder;
        protected ButtonStateRenderingHolder _pressedStateRenderingHolder;
        protected ButtonStateRenderingHolder _deactivatedStateRenderingHolder;

        public ButtonState _currentState { get; protected set; }
        readonly GameObject _linkedTo;

        public event EventHandler buttonClicked;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_linkedTo != null) _position = _linkedTo._position;

            DoStateBehaviour();
        }

        protected virtual void DoStateBehaviour() // WHO NEEDS A STATEMACHINE ANYWAY? YIPPEEEEE
        {
            ButtonState? newState;

            newState = _currentState switch
            {
                ButtonState.Interactable => InteractableBehaviour(),
                ButtonState.Hovering => HoveringBehaviour(),
                ButtonState.Held => HeldBehaviour(),
                ButtonState.Pressed => PressedBehaviour(),
                ButtonState.Deactivated => DeactivatedBehaviour(),
                _ => throw new System.Exception("FUCK")
            };

            if (newState != null)
            {
                ChangeState(newState ?? throw new System.Exception("This should not be possible to throw... I literally give this fuck a value in the previous line"));
            }
        }

        // If overlap, goto hovering or held
        protected virtual ButtonState? InteractableBehaviour()
        {
            if(!MouseController.GetMouseOverlap(this)) return null;

            return MouseController.GetButtonPressed(MouseButton.Left) ? ButtonState.Held : ButtonState.Hovering;
        }

        // If no overlap, back to interactable
        // If overlap and press, goto held
        protected virtual ButtonState? HoveringBehaviour()
        {
            if (!MouseController.GetMouseOverlap(this)) return ButtonState.Interactable;
            
            return MouseController.GetButtonPressed(MouseButton.Left) ? ButtonState.Held : null;
        }

        // Wait until left-mouse-button is no longer held
        protected virtual ButtonState? HeldBehaviour()
        {
            if (MouseController.GetButtonPressed(MouseButton.Left)) return null;

            return ButtonState.Pressed;
        }

        // Do click, return to interactable
        protected virtual ButtonState? PressedBehaviour()
        {
            OnClick();

            return _currentState == ButtonState.Deactivated ? null : ButtonState.Interactable;
        }

        // Do nothing
        protected virtual ButtonState? DeactivatedBehaviour()
        {
            return null;
        }

        protected virtual void ChangeState(ButtonState _newState)
        {
            _currentState = _newState;

            ButtonStateRenderingHolder newRenderingHolder = _currentState switch
            {
                ButtonState.Interactable => _interactableStateRenderingHolder,
                ButtonState.Hovering => _hoveringStateRenderingHolder,
                ButtonState.Held => _heldStateRenderingHolder,
                ButtonState.Pressed => _pressedStateRenderingHolder,
                ButtonState.Deactivated => _deactivatedStateRenderingHolder,
                _ => throw new System.Exception("FUCK")
            };

            _texture = newRenderingHolder._texture;
            _alpha = newRenderingHolder._alpha;
        }

        // These next two functions could be made virtual to allow for different default states I guess
        // Could be good in some edge-cases
        public void ActivateButton()
        {
            if (_currentState != ButtonState.Deactivated) return;
            ChangeState(ButtonState.Interactable); 
        }

        public void DeactivateButton()
        {
            if (_currentState == ButtonState.Deactivated) return;
            ChangeState(ButtonState.Deactivated);
        }

        public override void OnClick()
        {
            buttonClicked?.Invoke(this, EventArgs.Empty);
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

    public struct ButtonStateRenderingHolder
    {
        public ButtonStateRenderingHolder(Texture2D texture, float alpha = 1)
        {
            _texture = texture;
            _alpha = alpha;
        }

        public Texture2D _texture {get; private set;}
        public float _alpha { get; private set;}
    }
}