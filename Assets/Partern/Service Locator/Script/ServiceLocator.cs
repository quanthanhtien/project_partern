using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        private static Dictionary<Scene, ServiceLocator> sceneContainers;

        readonly ServiceManager services = new ServiceManager();

        const string k_gloablServiceLocatorName = "ServiceLocator [Global]";

        public static ServiceLocator Global
        {
            get
            {
                if (global != null)
                    return global;

                var container = new GameObject("Name", typeof(ServiceLocator));
                // container.AddComponent<ServiceLocatorGlobalBoostrapper>();
                return global;
            }
        }
    }
}
