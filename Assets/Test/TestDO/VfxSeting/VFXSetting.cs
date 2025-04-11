using System;
using n4;
using UnityEngine;

public class VFXSetting : MonoBehaviour
{
    public DataWeapon DataWeapon;
    private CountdownTimer timer;

    public void Start()
    {
        timer = new CountdownTimer(DataWeapon.lifeTime);
        timer.OnTimerStop += () => Destroy(gameObject);
        timer.Start();
        Debug.Log("VFXSetting OnEnable");
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
    }
}
