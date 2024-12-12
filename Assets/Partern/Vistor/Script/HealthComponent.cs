using Sirenix.OdinInspector;
using UnityEngine;

namespace Visitor
{
    public class HealthComponent : MonoBehaviour, IVisitable
    {
        [ShowInInspector]
        float Health = 100;

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            Debug.Log("HealthComponent.Accept");
        }

        public void AddHealth(float value)
        {
            Health += value;
        }
    }
}
