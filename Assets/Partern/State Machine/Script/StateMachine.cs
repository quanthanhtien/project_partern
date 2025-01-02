using System.Collections.Generic;

namespace Platformer
{
    public class StateMachine
    {
        StateNode current;
        
        public class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }
            HashSet<ITransition> anyTransition = new();

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }
            
            public void AddTransition(IState to,IPredicate condition)
            {
                Transitions.Add(new Transition(to,condition));
            }
        }
        
    }
}