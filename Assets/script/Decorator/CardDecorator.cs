using UnityEngine;

namespace CardGame
{
    public abstract class CardDecorator : ICard
    {
        protected ICard card;
        protected readonly int value;
        protected CardDecorator(int value)
        {
            this.value = value;
        }

        protected void Decorator(ICard card)
        {
            this.card = card;
        }
        public virtual int Play()
        {
            Debug.Log("card decorator"+ value);
            return card?.Play()+value?? value;
        }
    }

    public class DameDecorator : CardDecorator
    {
        public DameDecorator(int value) : base(value) {}
        public override int Play()
        {
            DamePlayer();
            return card?.Play()??0;
        }
        public void DamePlayer()
        {
            Debug.Log("Increase player dame" + value);
            // Increase player dame
        }
        
    }
    
    public class HealthDecorator : CardDecorator
    {
        public HealthDecorator(int value) : base(value) {}
        public override int Play()
        {
            HealthPlayer();
            return card?.Play()??0;
        }
        public void HealthPlayer()
        {
            Debug.Log("Increase player health" + value);
            // Increase player health
        }
    }
}