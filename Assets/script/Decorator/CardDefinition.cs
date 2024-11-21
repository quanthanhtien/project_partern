using Sirenix.OdinInspector;
using UnityEngine;

namespace script.Decorator
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards/CardDefinition"), ]
    public class CardDefinition : ScriptableObject
    {
        public int value;
        public CardType type = CardType.Battle;
        
    }
    public enum CardType
    {
        Battle,
        Damage,
        Health
    }
    

    public static class CardFactory
    {
        public static ICard Create(CardDefinition cardDefinition)
        {
            return cardDefinition.type switch
            {
                CardType.Health => new HealthDecorator(cardDefinition.value),
                CardType.Damage => new DameDecorator(cardDefinition.value),
                _ => new BattleCard(cardDefinition.value)
            };
        }
        
    }
}