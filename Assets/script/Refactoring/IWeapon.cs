using UnityEngine;
namespace script.Refactoring
{
    public interface IWeapon
    {
        public void Initialize(WeaponConfig weaponConfig);
        public void Equip();
        public void UnEquip();
        public void Use();
    }

    public abstract class WeaponBase : MonoBehaviour,IWeapon
    {
        protected string WeaponName;
        protected float Damage;
        public void Initialize(WeaponConfig weaponConfig)
        {
            this.WeaponName = weaponConfig.weaponName;
            this.Damage = weaponConfig.damage;
        }
        
        
        public void Equip()
        {
            Debug.Log($"{WeaponName} Equip");
        }

        public void UnEquip()
        {
            Debug.Log($"{WeaponName} UnEquip");
        }

        public abstract void Use();


    }
    
     
}
