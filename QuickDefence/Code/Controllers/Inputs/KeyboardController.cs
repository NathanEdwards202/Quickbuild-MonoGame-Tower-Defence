using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

// I wish I used the keyboard more than for just escape to end the game, I like this class

namespace Controllers.Inputs
{
    public static class KeyboardController
    {
        //public static Keys _pressedKeys { get; private set; }

        // Stores the duration for which each key is held
        // The key referenced by Keys[i] is the same as the key referenced by _keyPressDuration[i]
        // -1 represents having just been released this frame
        static readonly double[] _keyPressDuration = new double[Enum.GetValues(typeof(Keys)).Cast<int>().Max()];

        public static void Update(double deltaTime)
        {
            int lastIndex;
            int currentIndex = 0;

            foreach (Keys key in Keyboard.GetState().GetPressedKeys()) // Loop through keys
            {
                lastIndex = currentIndex; // Set last index to current index (before updating current index)
                currentIndex = (int)key; // Set current index to the index of the current key

                for (int i = lastIndex + 1; i < currentIndex; i++) UnpressedLogic(i); // Perform Unpressed logic on any currently unpressed keys between last index and current index

                _keyPressDuration[(int)key] += deltaTime; // Increase the currently pressed keys' duration by deltaTime
            }

            for (int i = currentIndex + 1; i < _keyPressDuration.Length; i++) UnpressedLogic(i); // Perform Unpressed logic on any currently unpressed keys between the last index and the final index
        }

        // If button is unpressed, set its value to 0
        // However, if button was released THIS FRAME, set its value to -1 (so we can check for it having just been released)
        static void UnpressedLogic(int index)
        {
            if (_keyPressDuration[index] == 0) return;

            else if (_keyPressDuration[index] == -1)
            {
                _keyPressDuration[index] = 0;
                return;
            }

            else
            {
                _keyPressDuration[index] = -1;
            }
        }


        public static bool GetKeyPressed(Keys key)
        {
            return _keyPressDuration[(int)key] > 0;
        }

        public static double GetKeyPressedTime(Keys key)
        {
            return _keyPressDuration[(int)key];
        }

        public static bool GetKeyUnpressed(Keys key)
        {
            return !GetKeyPressed(key);
        }

        public static bool GetKeyReleasedThisFrame(Keys key)
        {
            return _keyPressDuration[(int)key] == -1;
        }

        public static void ResetKey(Keys key)
        {
            _keyPressDuration[(int)key] = 0;
        }
    }
}
