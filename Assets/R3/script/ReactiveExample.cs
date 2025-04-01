using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;
using UnityEngine.InputSystem.HID;

public class ReactiveExample : MonoBehaviour
{
    public TMP_Text couterText;
    public Button cancellButton;
    public Button coinButton;
    public ParticleSystem coinParticle;
    
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
        coinButton.onClick.AddListener(() =>
        {
            subscription = Observable
                .Interval(TimeSpan.FromSeconds(1), cts.Token)
                .Subscribe(_ =>
                {
                    coinParticle.Play();
                });
        });
        
    }

    private void OnDestroy()
    {
        subscription?.Dispose();
        cts?.Dispose();
    }
}
