using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField, Required]
    WeaponFactory weaponFactory;

    [SerializeField, Required]
    ShieldFactory shieldFactory;
    IWeapon weapon = IWeapon.CreateDefault();
    IShield shield = IShield.CreateDefault();

    void Start()
    {
        weapon = weaponFactory.CreateWeapon();
        shield = shieldFactory.CreateShield();
        Attack();
        Defend();
    }

    public void Attack() => weapon?.Attack();

    public void Defend() => shield?.Defend();
}
