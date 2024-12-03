using System.Collections;
using UnityEngine;

namespace FlyWeight
{
    public class Projectile : Flyweight
    {
        new ProjectileSettings settings => (ProjectileSettings) base.settings;
        
        public void OnEnable()
        {
            StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
        }

        private void Update()
        {
            transform.Translate(Vector3.forward * (settings.speed * Time.deltaTime));
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helper.GetWaitForSeconds(delay);
            FlyweightFactory.ReturnToPool(this);
        }
    }
}