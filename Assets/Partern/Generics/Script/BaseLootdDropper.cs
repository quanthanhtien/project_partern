using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generics.Inheritance
{
    public abstract class BaseLootdDropper<TResolver>
        where TResolver : ILootResolver
    {
        protected readonly LootBag<Item> lootBag;
        TResolver resolver;

        public BaseLootdDropper(TResolver resolver)
        {
            this.resolver = resolver;
            lootBag = new LootBag<Item>();
        }

        public void DropLoot()
        {
            if (resolver.ShouldDropLoot())
            {
                OnDropLoot();
            }
            else
            {
                Debug.Log($"{GetType().Name} failed the loot resolve check");
            }
        }

        protected abstract void OnDropLoot();

        public void AddLoot(Item item)
        {
            lootBag.AddItem(item);
        }

        public class Builder
        {
            readonly List<Item> lootItems = new List<Item>();
            ILootResolver resolver;

            public Builder Reset()
            {
                resolver = null;
                lootItems.Clear();
                return this;
            }

            public Builder WithResolver(TResolver resolver)
            {
                this.resolver = resolver;
                return this;
            }

            public Builder WithLootItems(params Item[] items)
            {
                lootItems.AddRange(items);
                return this;
            }

            public T Build<T, TResolver>()
                where T : BaseLootdDropper<TResolver>
                where TResolver : ILootResolver
            {
                if (resolver == null)
                {
                    throw new InvalidOperationException(
                        "Resolver must be set before building a LootDropper"
                    );
                }

                if (resolver is not TResolver compatibleResolver)
                {
                    throw new InvalidOperationException(
                        $"Resolver is not compatible with {typeof(TResolver).Name}"
                    );
                }
                var dropper = (T)Activator.CreateInstance(typeof(T), compatibleResolver);
                if (dropper == null)
                {
                    throw new InvalidOperationException(
                        $"Could not create instance of {typeof(T).Name}"
                    );
                }

                foreach (var item in lootItems)
                {
                    dropper.AddLoot(item);
                }

                return dropper;
            }
        }
    }

    // public interface TResolver { }
}
