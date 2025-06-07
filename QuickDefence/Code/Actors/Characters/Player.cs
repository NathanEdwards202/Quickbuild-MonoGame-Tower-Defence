


// I am literally only in the Actor namespace because of my own idiocy when setting this stuff up.
// The towers are actors, the player is not

using System;

namespace Actors
{
    // Why not, these come from an unused and completely unnecessary idea
    // Another forgor to delete
    public enum PlayerMode
    {
        Default,
        Placing
    }

    internal static class Player
    {
        public static void OnGameStartSetup()
        {
            _lives = 100;
        }

        public static float _lives {get; private set;}

        public static event EventHandler onEnemyReachedEnd;

        public static void OnEnemyReachedEnd(float damage)
        {
            _lives -= damage;

            onEnemyReachedEnd?.Invoke(null, EventArgs.Empty); // Oh yeah, this isn't a confusing name for either the function or the event, not at all... Seriously though, wtf is happening here? I do not remember
        }
    }
}
