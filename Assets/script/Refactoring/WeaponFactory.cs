using UnityEngine;

namespace script.Refactoring
{
    public class WeaponFactory
    {
        public GameObject CreateWeapon(WeaponConfig config)
        {
            // Preconditions.CheckNotNull(config, "WeaponConfig is null");
            // Preconditions.CheckNotNull(config.prefabs, "Prefab is null");
            GameObject weaponInstance =  Object.Instantiate(config.prefabs);
            var configurableWeapon = weaponInstance.GetComponent(typeof(IWeapon)) as IWeapon;
            configurableWeapon?.Initialize(config);
            return weaponInstance;
        }
    }
}