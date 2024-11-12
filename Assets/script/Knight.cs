using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField, Required]
    public EquipmentFactory equipmentFactory;

    [SerializeField, Required] SpellStrategy[] spells;
    void Start()
    {
        Attack();
        Defend();
    }

    public void Attack() => equipmentFactory?.CreateWeapon().Attack();

    public void Defend() => equipmentFactory?.CreateShield().Defend();
    
    void OnEnable()
    {
        HeatsUpDisplay.OnButtonPressed += CastSpell;
    }
    void OnDisable()
    {
        HeatsUpDisplay.OnButtonPressed -= CastSpell;
    }
    void CastSpell(int index)
    {
        spells[index].CastSpell(transform);
    }
}
