namespace Generics.Inheritance
{
    public class LootDropperFactory
    {
        private readonly BaseLootdDropper<ILootResolver>.Builder builder = new();

        public EnemyLootDropper CreateEnemyLootDropper(Item[] items, float dropChance = 0.5f)
        {
            return builder
                .WithResolver(new DropChanceResolver(dropChance))
                .WithLootItems(items)
                .Build<EnemyLootDropper, ILootResolver>();
        }

        public ChestLoopDropper CreateChestLootDropper(
            Item[] items,
            string enviromentType = "Dungeon"
        )
        {
            return builder
                .WithResolver(new ChestSpecificResolver(enviromentType))
                .WithLootItems(items)
                .Build<ChestLoopDropper, ChestSpecificResolver>();
        }
    }
}
