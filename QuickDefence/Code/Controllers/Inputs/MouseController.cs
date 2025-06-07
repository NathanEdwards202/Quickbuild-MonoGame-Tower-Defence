using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Scenes.Objects;
using System;
using System.Collections.Generic;

namespace Controllers.Inputs
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class MouseController
    {
        static MouseState _mouse;

        static Dictionary<MouseButton, double> _holdDuration;
        static int _oldScroll;
        static int _scrollThisFrame;

        public static void Initialize()
        {
            _holdDuration = new()
            { 
                {MouseButton.Left, 0 },
                {MouseButton.Right, 0 },
                {MouseButton.Middle, 0 },
            };

            _oldScroll = 0;
            _scrollThisFrame = 0;
        }

        public static void Update(double deltaTime)
        {
            _mouse = Mouse.GetState();

            UpdateButtonDuration(MouseButton.Left, _mouse.LeftButton, deltaTime);
            UpdateButtonDuration(MouseButton.Right, _mouse.RightButton, deltaTime);
            UpdateButtonDuration(MouseButton.Middle, _mouse.MiddleButton, deltaTime);


            if (_mouse.ScrollWheelValue != _oldScroll)
            {
                _scrollThisFrame = _mouse.ScrollWheelValue - _oldScroll;
                _oldScroll = _mouse.ScrollWheelValue;
            }
        }

        static void UpdateButtonDuration(MouseButton btn, ButtonState btnState,double deltaTime)
        {
            double duration = _holdDuration[btn];
            _holdDuration[btn] = (btnState == ButtonState.Pressed) ? duration + deltaTime : 0;
        }

        public static Vector2 GetPosition()
        {
            return new(_mouse.Position.X, _mouse.Position.Y);
        }

        public static double GetButtonHoldDuration(MouseButton btn) => _holdDuration[btn];
        public static bool GetButtonPressed(MouseButton btn) => _holdDuration[btn] > 0;
        public static bool GetButtonReleased(MouseButton btn) => _holdDuration[btn] == 0;

        public static int GetScrollAmountThisFrame() => _scrollThisFrame;
        public static bool GetMouseScroll() => (_scrollThisFrame != 0);



        public static bool GetMouseOverlap(GameObject obj)
        {
            Vector2 mousePos = new(_mouse.Position.X, _mouse.Position.Y);

            return mousePos.X >= obj._position.X && mousePos.X <= obj._position.X + obj._size.X
                && mousePos.Y >= obj._position.Y && mousePos.Y <= obj._position.Y + obj._size.Y;
        }

        public static bool GetMouseOverlap(Rectangle rect)
        {
            Vector2 mousePos = new(_mouse.Position.X, _mouse.Position.Y);

            return mousePos.X >= rect.X && mousePos.X <= rect.X + rect.Width
                && mousePos.Y >= rect.Y && mousePos.Y <= rect.Y + rect.Height;
        }

        // Yeah... Unused :)
        public static bool GetMouseOverlapRotationConsidered(GameObject obj)
        {
            // Assuming _mouse.Position is in screen space and obj._position is in the same space
            Vector2 mousePos = new(_mouse.Position.X, _mouse.Position.Y);

            // Calculate the difference between the mouse position and the object position
            Vector2 mouseOffset = mousePos - obj._position;

            // Inverse rotate the mouse position into the object's local space
            float angle = obj._rotation; // Object's rotation in radians
            float cos = MathF.Cos(-angle); // Negative because we want to reverse the rotation
            float sin = MathF.Sin(-angle);

            // Apply the inverse rotation matrix to the mouseOffset to get the mouse position in the object's local space
            Vector2 rotatedMousePos = new(
                mouseOffset.X * cos - mouseOffset.Y * sin,
                mouseOffset.X * sin + mouseOffset.Y * cos
            );

            // Check if the rotated mouse position is within the bounds of the object's local space
            return rotatedMousePos.X >= -obj._size.X / 2 && rotatedMousePos.X <= obj._size.X / 2
                && rotatedMousePos.Y >= -obj._size.Y / 2 && rotatedMousePos.Y <= obj._size.Y / 2;
        }

    }
}