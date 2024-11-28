using UnityEngine;

namespace script.Refactoring
{
    public enum WeaponType
    {
        Sword,
        Bow,
        Staff
    }
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Items/WeaponConfig")]
    public class WeaponConfig : ScriptableObject
    {
        public string weaponName;
        public float damage;
        public WeaponType weaponType;
        public float range;
        public GameObject prefabs;
        public Sprite icon;
    }
}