using System;
using UnityEngine;

namespace R3
{
    public class Player : MonoBehaviour
    {
        public event Action<int> OnPlayerDamaged;

        public int maxHp = 150;
        
        public ReactiveProperty<int> CurrentHp { get; private set; }
        public ReactiveProperty<bool> IsDead { get; private set; }

        private void Start()
        {
            CurrentHp = new ReactiveProperty<int>(maxHp);
            IsDead = new ReactiveProperty<bool>(false);

            CurrentHp.Subscribe(hp => IsDead.Value = hp <= 0)
                .AddTo(this);
        }

        public void TakeDamage(int damage)
        {
            OnPlayerDamaged?.Invoke(damage);
        }
    }
}