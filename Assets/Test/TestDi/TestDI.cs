using UnityEngine;

namespace DependencyInjection
{
    public class TestDI : MonoBehaviour
    {
        [Inject] ServiceA serviceA; 
    
        public void Start()
        {
            serviceA.DoSomething();
        }
    }

    public interface IServiceA
    {
        void DoSomething(); 
    }
}
