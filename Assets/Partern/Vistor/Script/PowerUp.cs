using System;
using UnityEngine;

namespace Visitor
{
    [CreateAssetMenu(fileName = "PowerUp", menuName = "Visitor/PowerUp")]
    public class PowerUp : ScriptableObject, IVisitor
    {
        public int HealthBonus;
        public int ManaBonus;

        public void Visit(HealthComponent healthComponent)
        {
            healthComponent.AddHealth(HealthBonus);
            Debug.Log("PowerUp.Visit(HealthComponent)");
        }

        public void Visit(ManaComponent manaComponent)
        {
            manaComponent.AddMana(ManaBonus);
            Debug.Log("PowerUp.Visit(ManaComponent)");
        }
    }
}
