    using System;
using System.Reflection;
using UnityEngine;

namespace Visitor
{
    [CreateAssetMenu(fileName = "PowerUp", menuName = "Visitor/PowerUp")]
    public class PowerUp : ScriptableObject, IVisitor
    {
        public int HealthBonus;
        public int ManaBonus;

        public void Visit(object o)
        {
            MethodInfo visitMethod = GetType().GetMethod("Visit", new[] { o.GetType() });
            if (
                visitMethod != null
                && visitMethod != GetType().GetMethod("Visit", new[] { typeof(object) })
            )
            {
                visitMethod.Invoke(this, new[] { o });
            }
            else
            {
                DefaultVisit(o);
            }
        }

        void DefaultVisit(object o)
        {
            Debug.Log("PowerUp.DefaultVisit");
        }

        // public void Visit(HealthComponent healthComponent)
        // {
        //     healthComponent.AddHealth(HealthBonus);
        //     Debug.Log("PowerUp.Visit(HealthComponent)");
        // }

        // public void Visit(ManaComponent manaComponent)
        // {
        //     manaComponent.AddMana(ManaBonus);
        //     Debug.Log("PowerUp.Visit(ManaComponent)");
        // }
    }
}
