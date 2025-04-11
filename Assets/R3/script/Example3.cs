using System;
using UnityEngine;
using R3;
using UnityEngine.UI;

public class Example3 : MonoBehaviour
{
    public Slider healthSlider;

    void Start()
    {
        // Health slider monitoring (unchanged)
        healthSlider.OnValueChangedAsObservable()
            .Where(value => value < 0.3f)
            .Subscribe(value => Debug.Log($"Low health: {value}"))
            .AddTo(this);
    }
}