using System;
using UnityEngine;

public class Solider : MonoBehaviour
{
    [SerializeField]
    public EquipmentFactory equipmentFactory;

    void Start()
    {
        Attack();
        Defend();
    }

    public void Attack() => equipmentFactory?.CreateWeapon().Attack();

    public void Defend() => equipmentFactory?.CreateShield().Defend();
}
