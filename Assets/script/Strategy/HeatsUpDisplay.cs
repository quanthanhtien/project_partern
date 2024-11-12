using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeatsUpDisplay : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    public delegate void ButtonPrssedEvent(int index);
    public static event ButtonPrssedEvent OnButtonPressed;

    private void Awake()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => HandleButtonPress(index));
        }
    }
    void HandleButtonPress(int index) => OnButtonPressed?.Invoke(index);
}
