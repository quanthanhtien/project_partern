using System;
using UnityEngine;

namespace ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Boostrapper : MonoBehaviour
    {
        ServiceLocator container;
        internal ServiceLocator Container =>
            container ?? (container = GetComponent<ServiceLocator>());

        bool hasBeenBootstrapped;

        private void Awake()
        {
            BootstrapOnDemand();
        }

        public void BootstrapOnDemand()
        {
            if (hasBeenBootstrapped)
                return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();

        [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
        public class ServiceLocatorGlobalBootstrapper : Boostrapper
        {
            [SerializeField]
            bool dontDestroyOnload = true;

            protected override void Bootstrap() { }
        }

        [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
        public class ServiceLocatorSceneBootstrapper : Boostrapper
        {
            protected override void Bootstrap() { }
        }
    }
}
