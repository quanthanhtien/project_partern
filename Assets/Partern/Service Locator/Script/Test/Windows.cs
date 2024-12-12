using UnityEngine;

namespace Partern.Service_Locator.Script
{
    public class Windows : IOperatingSystem
    {
        public  Windows()
        {
            Name = "Windows XP";
        }

        public string Name { get; set; }

        public void Run()
        {
            Debug.Log($"{Name} is running.");
        }
        
    }
    public interface IOperatingSystem
    {
        string Name { get; set; }
        void Run();
    }

    public class Computer
    {
        private readonly IOperatingSystem _operatingSystem;

        public Computer()
        {
            _operatingSystem = ServiceLocator.Instance.GetService<IOperatingSystem>();
        }

        public void Start()
        {
            _operatingSystem.Run();
        }
    }
}