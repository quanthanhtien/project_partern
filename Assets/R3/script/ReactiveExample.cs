using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;

namespace R3
{
    public class ReactiveExample : MonoBehaviour
    {
        public TMP_Text couterText;
        public Button cancellButton;
        public Button coinButton;
        public ParticleSystem coinParticle;
        public Player player;
        
        
        IDisposable subscription;
        CancellationTokenSource cts;

        private void Awake()
        {
            couterText.text = "0";
            cts = new CancellationTokenSource();
            cancellButton.onClick.AddListener(()=>cts.Cancel());
        }
    
        void Start()
        {
            Observable.FromEvent<int>(
                handler => player.OnPlayerDamaged += handler,
                handler => player.OnPlayerDamaged += handler
            ).Subscribe(damage => Debug.Log($"Player took {damage} damage")).AddTo(this);
            
            player.CurrentHp.Subscribe(x => Debug.Log($"HP: {x}") ).AddTo(this);
            player.IsDead.Where(isDead => isDead == true).Subscribe(_=>coinButton.enabled = false).AddTo(this);
            
            subscription = coinButton.OnClickAsObservable().Subscribe(_ => 
            {
                player.TakeDamage(99);
                coinParticle.Play();
            });
        }

        private void OnDestroy()
        {
            subscription?.Dispose();
            cts?.Dispose();
        }
    }
}

