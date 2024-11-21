using System;

namespace script.Decorator
{
    public class CardManager : Singleton<CardManager>
    {
        public static CardManager Instance;
        public CardController SelectedCard;
        private void Awake()
        {
            Instance = this;
        }
    }
}