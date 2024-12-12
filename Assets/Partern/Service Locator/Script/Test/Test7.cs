using UnityEngine;

namespace Partern.Service_Locator.Script
{
    public class Test7 : MonoBehaviour
    {
        public void Start()
        {
            ServiceLocator.Instance.Register<IOperatingSystem>(new Windows());

            Computer computer = new Computer();
            computer.Start();

        }
    }
}
