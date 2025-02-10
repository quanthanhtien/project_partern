using System;
using UnityEngine;
using UnityServiceLocator;

namespace Partern.Mediator.Script
{
    public class AgentMediator : Mediator<Agent>
    {
        private void Awake()
        {
            ServiceLocator.Global.Register(this as Mediator<Agent>);
        }

        protected override void OnDeregistered(Agent entity)
        {
            Debug.Log($"{entity.name} has been deregistered");
            Broadcast(entity, new MessagePayload { Source = entity, Content = "Registered" });
        }

        protected override void OnRegistered(Agent entity)
        {
            Debug.Log($"{entity.name} has been registered");
            Broadcast(entity, new MessagePayload { Source = entity, Content = "Deregister" });
        }

        protected override bool MediatorConditionMet(Agent target)
        {
            return target.Status == AgentStatus.Active;
        }
    }
}
