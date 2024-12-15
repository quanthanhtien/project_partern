using UnityEngine;

namespace Partern.Mediator.Script
{
    public abstract class Payload<TData> : IVisitor
    {
        public abstract TData Content { get; set; }
        public abstract void Visit<T>(T visitable) where T : Component, IVisitable;
    }
    
    public class MessagePayload : Payload<string>
    {
        public Agent Source { get; set; }
        public override string Content { get; set; }
        public override void Visit<T>(T visitable)
        {
            Debug.Log($"{visitable.name} received message from {Source.name}: {Content}");
        }
    }
}