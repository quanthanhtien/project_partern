using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Sirenix.Utilities;

namespace Partern.Mediator.Script
{
    public abstract class Mediator<T> : MonoBehaviour where T : Component, IVisitable
    {
        private List<T> _entities = new List<T>();

        public void Register(T entity)
        {
            if (!_entities.Contains(entity))
            {
                _entities.Add(entity);
            }
        }

        public void DeRigister(T entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
            }
        }

        public void Message(T source, T target, IVisitor message)
        {
            _entities.FirstOrDefault(entity => entity.Equals(target))?.Accept(message);
        }

        public void Broadcast(T source, IVisitor message, Func<T, bool> predicate = null)
        {
            _entities.Where(target => source != target && SenderConditionMet(target,predicate) && MeidiatorConditionMet(target))
                .ForEach(target => target.Accept(message));
        }

        bool SenderConditionMet(T target, Func<T, bool> predicate) => predicate == null || predicate(target);
        protected abstract bool MeidiatorConditionMet(T target);
    }
}

