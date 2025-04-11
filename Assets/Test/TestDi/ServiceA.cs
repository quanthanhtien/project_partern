using UnityEngine;

namespace DependencyInjection
{
    public class ServiceA : MonoBehaviour, IDependencyProvider
    {
        public void DoSomething()
        {
            Debug.Log("ServiceA is doing something!");
        }

        [Provide]
        public ServiceA ProvideServiceA()
        {
            return this;
        }
    }
}