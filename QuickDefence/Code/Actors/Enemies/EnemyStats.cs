using Misc;
using System;
using System.Diagnostics;

namespace Actors.Enemies
{
    internal class EnemyStats : ActorStats
    {
        public EnemyStats(EnemyStatsTemplate template) 
            : base(template._name, AssetManager.GetTexture(template._textureName), template._rotationSpeed)
        {
            _maxHP = template._hp;
            _currentHp = template._hp;
            _speed = template._spd;
            _damage = template._dmg;

            if (this.GetType() == typeof(EnemyStats)) Debug.WriteLine("EnemyStats initialized.");
        }

        ~EnemyStats()
        {
            onDamageTakenSendPercentHP = null;
        }

        public readonly float _maxHP;
        public float _currentHp { get; private set; }
        public float _speed { get; private set; }
        public float _damage { get; private set; }


        public event EventHandler<OnDamageTakenEventArgs> onDamageTakenSendPercentHP; // This was part of me learning C# events... Please don't blame me for this being SOOOO unnecessarily bad design
        public class OnDamageTakenEventArgs : EventArgs
        {
            public float damage;
            public float percentHP;
        }

        public event EventHandler onOutOfHP;

        public void TakeDamage(float damage)
        {
            _currentHp -= damage;

            if (_currentHp <= 0) onOutOfHP?.Invoke(null, null);

            onDamageTakenSendPercentHP?.Invoke(this, new OnDamageTakenEventArgs() { damage = damage, percentHP = _currentHp / _maxHP });
        }
    }
}
