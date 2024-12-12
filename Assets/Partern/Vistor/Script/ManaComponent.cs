using Sirenix.OdinInspector;
using UnityEngine;

namespace Visitor
{
    public class ManaComponent : MonoBehaviour, IVisitable
    {
        [ShowInInspector]
        float Mana = 100;

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Debug.Log("ManaComponent.Accept");
        }

        public void AddMana(float value)
        {
            Mana += value;
        }
    }
}
