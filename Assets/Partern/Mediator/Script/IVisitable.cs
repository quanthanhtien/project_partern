
using UnityEngine;

namespace Partern.Mediator.Script
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor) ;
    }
    
    public interface IVisitor
    {
        void Visit<T>(T visitable) where T : Component, IVisitable;
    }
}