using UnityEngine;
using Sirenix.OdinInspector;
namespace ServiceLocator
{
    public class TestService : MonoBehaviour
    {
        public IWeapon weapon;
        private void Awake()
        {
            ServiceLocator.Global.Register<IWeapon>(weapon = new Weapon());
        }
        
        [Button]
        public void ShowWeapon()
        {
            ServiceLocator.For(this).Get(out weapon);
            weapon.show();
        }
    }

    public interface IWeapon
    {
        void show();
    }
    public class Weapon : IWeapon
    {
        public void show()
        {
            Debug.Log("Weapon");
        }
    }
}