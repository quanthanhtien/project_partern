using UnityEngine;

namespace script.Refactoring
{
    public class WeaponController :MonoBehaviour
    {
        
        GameObject currentWeapon;
        public Transform mountPoint;

        private WeaponFactory factory = new();
        public void EquipWeapon(WeaponConfig config)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }
            
            currentWeapon = factory.CreateWeapon(config);
            currentWeapon.transform.SetParent(mountPoint);
        }
        
    }
}