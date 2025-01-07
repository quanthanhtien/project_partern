using System.Collections.Generic;
using UnityEngine;

namespace Generics.Inheritance
{
    abstract class Spawner<T>
        where T : Enemy
    {
        public abstract T Spawn();
    }

    class MeleeEnemySpawner : Spawner<MeleeEnemy>
    {
        public override MeleeEnemy Spawn()
        {
            Debug.Log("Spawned a melee enemy");
            return new MeleeEnemy();
        }
    }

    class RangedEnemySpawner : Spawner<RangedEnemy>
    {
        public override RangedEnemy Spawn()
        {
            Debug.Log("Spawned a ranged enemy");
            return new RangedEnemy();
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public int Quantity { get; set; }

        public Item(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public override string ToString() => $"{Quantity} x {Name}";
    }

    public class LootBag<T>
        where T : Item
    {
        List<T> items = new List<T>();

        public void AddItem(T item)
        {
            items.Add(item);
            Debug.Log($"Added: {item}");
        }

        public IEnumerable<T> GetAllItems() => items;
    }
}
