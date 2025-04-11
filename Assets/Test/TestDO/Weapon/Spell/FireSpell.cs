using System;
using System.Collections;
using UnityEngine;

namespace n4
{
    [CreateAssetMenu(fileName = "FireSpell", menuName = "Spells/FireSpell")]
    public class FireSpell : SpellStrategy
    {
        public override void CashSpell(Vector3 pos, GameObject vfx, GameObject target)
        {
            Instantiate(vfx, pos, Quaternion.identity);
        }
    }
}
