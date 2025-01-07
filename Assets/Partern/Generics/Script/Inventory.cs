using System.Collections.Generic;

namespace Generics.Inheritance
{
    class Inventory<T>
    {
        List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public void Remove(T item) => items.Remove(item);

        public int Count() => items.Count;
    }

    class HealthPotion { }

    class Weapon { }

    // Inventory<HealthPotion> healthPotionsInventory = new Inventory<HealthPotion>();
    // Inventory<Weapon> weaponsInventory = new Inventory<Weapon>();
}
