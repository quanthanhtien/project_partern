using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visitor
{
    public class Hero : MonoBehaviour, IVisitable
    {
        private List<IVisitable> visitableComponents = new List<IVisitable>();

        private void Start()
        {
            visitableComponents.Add(gameObject.GetComponent<HealthComponent>());
            visitableComponents.Add(gameObject.GetComponent<ManaComponent>());
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var visitableComponent in visitableComponents)
            {
                visitableComponent.Accept(visitor);
            }
        }
    }
}
