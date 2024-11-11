using UnityEngine;

public class Solider : MonoBehaviour
{
    [SerializeField]
    public EquipmentFactory equipmentFactory;
    

    public void Attack() => equipmentFactory?.CreateWeapon();

    public void Defend() => equipmentFactory?.CreateShield();
}
