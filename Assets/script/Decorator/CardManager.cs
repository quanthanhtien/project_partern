using System;
using UnityEngine;

namespace script.Decorator
{
    public class CardManager : Singleton<CardManager>
    {
        public CardController selectedCard;

        public void Decorate(CardController clickedCard)
        {
            if (selectedCard == clickedCard)
                return;
            if (selectedCard.Card is CardDecorator decorator)
            {
                decorator.Decorate(clickedCard.Card);
                clickedCard.Card = decorator;
                selectedCard.MoveToAndDestroy(clickedCard.transform.position);
            }
            else
            {
                Debug.LogWarning("can not decorate card");
            }
        }
    }
}
