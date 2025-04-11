using System;
using UnityEngine;

namespace n4
{
    public class Weapon : MonoBehaviour
    {
        public DataWeapon DataWeapon;
        float damage;
        float lifeTime;
        float coolDown;
        GameObject vfx;

        private void OnEnable()
        {
            Init();
        }

        void Init()
        {
            damage = DataWeapon.damage;
            lifeTime = DataWeapon.lifeTime;
            coolDown = DataWeapon.coolDown;
            vfx = DataWeapon.vfx;
        }

        private void OnTriggerEnter(Collider other)
        {
            DataWeapon.effectSkill.CashSpell(other.transform.position, vfx, other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
