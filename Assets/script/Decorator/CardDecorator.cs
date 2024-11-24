using UnityEngine;

namespace script.Decorator
{
    public abstract class CardDecorator : ICard
    {
        protected ICard card;
        protected readonly int value;

        public CardDecorator(int value)
        {
            this.value = value;
        }

        public void Decorate(ICard card)
        {
            if (ReferenceEquals(this, card))
            {
                Debug.LogWarning("Can not decorate card");
                return;
            }
            if (this.card is CardDecorator decorator)
            {
                decorator.Decorate(card);
            }
            else
            {
                this.card = card;
            }
        }

        public virtual int Play()
        {
            return card?.Play() + value ?? value;
        }
    }

    public class DameDecorator : CardDecorator
    {
        public DameDecorator(int value)
            : base(value) { }

        public override int Play()
        {
            DamePlayer();
            return card?.Play() ?? 0;
        }

        public void DamePlayer()
        {
            Debug.Log("Playing DameDecorator card with value" + value);
            // Increase player dame
        }
    }

    public class HealthDecorator : CardDecorator
    {
        public HealthDecorator(int value)
            : base(value) { }

        public override int Play()
        {
            HealthPlayer();
            return card?.Play() ?? 0;
        }

        public void HealthPlayer()
        {
            Debug.Log("Increase player health" + value);
            // Increase player health
        }
    }

    public class ShieldDecorator : CardDecorator
    {
        public ShieldDecorator(int value)
            : base(value) { }

        public override int Play()
        {
            ShieldPlayer();
            return card?.Play() ?? 0;
        }

        public void ShieldPlayer()
        {
            Debug.Log("Increase player shield" + value);
            // Increase player shield
        }
    }
}
