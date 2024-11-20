namespace CardGame
{
    public class BattleCard : ICard
    {
        int value;

        public int Play()
        {
            return value;
        }
        
        public BattleCard(int value)
        {
            this.value = value;
        }
        
    }
}