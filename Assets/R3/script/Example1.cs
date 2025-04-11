using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Example1 : MonoBehaviour
{
    public ReactiveProperty<bool> isReloading = new(false);
    public ReactiveProperty<float> reloadProgress = new(0f);
    public ReactiveProperty<int> numberBullet = new(0);
    public Slider reloadSlider;
    public TMP_Text bulletText;
    private void Start()
    {
        UpdateNumberBullet();
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Space) && isReloading.Value == false)
            .SelectMany(_ =>
            {
                isReloading.Value = true;
                return Observable.Timer(TimeSpan.FromSeconds(2))
                    .Do(_ =>
                    {
                        Debug.Log("Reload complete");
                        AddBullet(10);
                        isReloading.Value = false;
                    });
            }).Subscribe().AddTo(this);
        
        Observable.EveryUpdate()
            .Where(_ => isReloading.Value == true)
            .Subscribe(_ => {
                reloadProgress.Value += Time.deltaTime / 2f;
                reloadSlider.value = reloadProgress.Value;
                if (reloadProgress.Value >= 1f) reloadProgress.Value = 0;
            }).AddTo(this);

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0) && numberBullet.Value > 0)
            .Subscribe(_ =>
            {
                SubBullet(1);
            }).AddTo(this);
    }

    void AddBullet(int bullet)
    {
        numberBullet.Value += bullet;
        UpdateNumberBullet();
    }

    void SubBullet(int bullet)
    {
        numberBullet.Value -= bullet;
        UpdateNumberBullet();
    }
    
    void UpdateNumberBullet()
    {
        bulletText.text = $"Number Bullet: {numberBullet.Value.ToString()}";
    }

}
