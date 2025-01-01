using System;

namespace Platformer
{
    public class Predicate
    {
        readonly Func<bool> func;

        public Predicate(Func<bool> func)
        {
            this.func = func;
        }
        
        public bool Evaluete() => func.Invoke();
    }
}