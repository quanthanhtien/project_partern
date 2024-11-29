using System;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Observer<int> Health = new Observer<int>(100);

    void Start()
    {
        Health.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Health.Value += 10;
        }
    }
}