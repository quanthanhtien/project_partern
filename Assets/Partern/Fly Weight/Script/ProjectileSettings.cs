using UnityEngine;

namespace FlyWeight
{
    [CreateAssetMenu(menuName = "FlyWeight/Projectile Settings")]
    public class ProjectileSettings : FlyweightSetting
    {
        public float despawnDelay = 5f;
        public float speed = 5f;
        public float damage = 10f;
        public override Flyweight CreateFlyweight()
        {
            Debug.Log("Create Projectile");
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name;
            var flyweight = go.AddComponent<Projectile>();
            flyweight.settings = this;
            return flyweight;
        }
        public override void OnGet(Flyweight f) => f.gameObject.SetActive(true);
        public override void OnRelease(Flyweight f) => f.gameObject.SetActive(false);
        public override void OnDestroyPoolObject(Flyweight f) => Destroy(f.gameObject);
    }
}