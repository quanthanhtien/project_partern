using System;
using UnityEngine;
using ServiceLocator;

// [RequireComponent(typeof(AgentMotor))]
namespace Partern.Mediator.Script
{
    public class Agent : MonoBehaviour, IVisitable
    {
        // IGoapMultithread goapSystem;
        // AgentMotor motor;
        Mediator<Agent> mediator;

        // public AgentStatus Status { get; set; } = AgentStatus.Active;

        private void Start()
        {
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}