using System;
using Cysharp.Threading.Tasks;
using R3.Triggers;
using UnityEngine;

namespace R3
{
    public class Player : MonoBehaviour
    {
        public event Action<int> OnPlayerDamaged;

        public int maxHp = 150;
        public SerializableReactiveProperty<int> playerScore = new(0);
        public ReactiveProperty<int> CurrentHp { get; private set; }
        public ReactiveProperty<bool> IsDead { get; private set; }

        private void Awake()
        {
            CurrentHp = new ReactiveProperty<int>(maxHp);
            IsDead = new ReactiveProperty<bool>(false);
        
            CurrentHp.Subscribe(hp => IsDead.Value = hp <= 0)
                .AddTo(this);
            this.OnCollisionEnterAsObservable()
                .Where(col => col.gameObject.CompareTag("enemy"))
                .Do(collision => Debug.Log("Player collided with enemy"))
                .Select(col => col.contacts[0].point)
                .Subscribe(CollisionPoint =>
                {
                    Debug.Log($"Collision point: {CollisionPoint}");
                    playerScore.Value += 10;
                }).AddTo(this);
            playerScore.Subscribe(score => Debug.Log($"score: {score}")).AddTo(this);
        }

        public void TakeDamage(int damage)
        {
            OnPlayerDamaged?.Invoke(damage);
            CurrentHp.Value -= damage;
        }
    }
}