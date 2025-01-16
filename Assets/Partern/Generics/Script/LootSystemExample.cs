using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generics.Inheritance
{
    public class LootSystemExample : MonoBehaviour
    {
        LootDropperFactory factory = new();

        void Start()
        {
            var enemyLootDropper = factory.CreateEnemyLootDropper(
                new[] { new Item("Sword", 1), new Item("Health Potion", 1) }
            );

            var chestLootDropper = factory.CreateChestLootDropper(
                new[] { new Item("Gold", 100), new Item("Mana Potion", 2) }
            );

            enemyLootDropper.DropLoot();
            chestLootDropper.DropLoot();
        }
    }
}
