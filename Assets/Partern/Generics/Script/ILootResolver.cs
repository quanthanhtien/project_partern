using UnityEngine;
using Random = UnityEngine.Random;

namespace Generics.Inheritance
{
    public interface ILootResolver
    {
        bool ShouldDropLoot();
    }

    public class DropChanceResolver : ILootResolver
    {
        readonly float dropChance;

        public DropChanceResolver(float dropChance)
        {
            this.dropChance = dropChance;
        }

        public bool ShouldDropLoot()
        {
            return Random.value <= dropChance;
        }
    }

    public interface ILockable
    {
        bool IsLocked { get; }
    }

    public class ChestSpecificResolver : ILootResolver, ILockable
    {
        readonly string enviromentType;
        public bool IsLocked => false;

        public ChestSpecificResolver(string enviromentType)
        {
            this.enviromentType = enviromentType;
        }

        public bool ShouldDropLoot()
        {
            return !IsLocked && enviromentType == "Dungeon"
                ? Random.value <= 0.5f
                : Random.value <= 0.4f;
        }
    }

    public class EnemyLootDropper : BaseLootdDropper<ILootResolver>
    {
        public EnemyLootDropper(DropChanceResolver resolver)
            : base(resolver)
        {
            Debug.Log("Enemy Loot Dropper created");
        }

        protected override void OnDropLoot()
        {
            Debug.Log("Enemy dropped loot");
            foreach (var item in lootBag.GetAllItems())
            {
                Debug.Log(item);
            }
        }
    }

    public class ChestLoopDropper : BaseLootdDropper<ChestSpecificResolver>
    {
        public ChestLoopDropper(ChestSpecificResolver resolver)
            : base(resolver)
        {
            Debug.Log("Chest Loot Dropper created");
        }

        protected override void OnDropLoot()
        {
            Debug.Log("Chest contains");
            foreach (var item in lootBag.GetAllItems())
            {
                Debug.Log(item);
            }
        }
    }
}
