using System;
using UnityEngine;

namespace n4
{
    public class Weapon : MonoBehaviour
    {
        public AbilityDataWeapon abilityDataWeapon;
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
            damage = abilityDataWeapon.damage;
            lifeTime = abilityDataWeapon.lifeTime;
            coolDown = abilityDataWeapon.coolDown;
            vfx = abilityDataWeapon.vfx;
        }

        private void OnTriggerEnter(Collider other)
        {
            abilityDataWeapon.effectSkill.CashSpell(
                other.transform.position,
                vfx,
                other.gameObject
            );
            Destroy(this.gameObject);
        }
    }
}
