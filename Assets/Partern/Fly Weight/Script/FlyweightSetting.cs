using UnityEngine;

namespace FlyWeight
{
    public class FlyweightSetting : ScriptableObject
    {
        public FlyweightType type;
        public GameObject prefab;

        public virtual Flyweight CreateFlyweight()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name;
            var flyweight = go.AddComponent<Flyweight>();
            flyweight.settings = this;
            return flyweight;
        }
        
        public virtual void OnGet(Flyweight f) => f.gameObject.SetActive(true);
        public virtual void OnRelease(Flyweight f) => f.gameObject.SetActive(false);
        public virtual void OnDestroyPoolObject(Flyweight f) => Destroy(f.gameObject);
    }

    public enum FlyweightType
    {
        Fire,
        Ice
    }
}