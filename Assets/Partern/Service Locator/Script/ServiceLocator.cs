using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

namespace ServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        private static Dictionary<Scene, ServiceLocator> sceneContainers;

        readonly ServiceManager services = new ServiceManager();

        const string k_gloablServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (global == this)
            {
                Debug.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global", this);
            } else if (global != this)
            {
                Debug.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global", this);
            }
            else
            {
                global = this;
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }
        
        internal void ConfigureForScene(Scene scene)
        {
            Scene currentScene = gameObject.scene;

            if (sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
            }
            
            sceneContainers.Add(scene, this);
        }
        
        public static ServiceLocator Global
        {
            get
            {
                if (global != null)
                    return global;
                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is {} found)
                {
                    global = found.GetComponent<ServiceLocator>();
                    return global;
                }
                
                var container = new GameObject(k_gloablServiceLocatorName, typeof(ServiceLocator));
                // container.AddComponent<ServiceLocatorGlobalBoostrapper>();
                return global;
            }
        }
        static List<GameObject> tmpSceneGameObjects;

        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponent<ServiceLocator>().OrNull() ?? ForSceneOf(mb) ?? Global;
        }
        
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            Scene scene = mb.gameObject.scene;
            
            if (sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != null)
                return container;
            
            tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(tmpSceneGameObjects);
            
            foreach(GameObject go in tmpSceneGameObjects.Where(go=>go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorGlobalBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }
        
        public ServiceLocator Register<T>(T service, bool isGlobal)
        {
            services.Register(service);
            return this;
        }
        
        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (services.TryGet(out service))
                return this;
            if (TryGetNextInHierachy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }
            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }
        
        bool TryGetService<T>(out T service) where T : class
        {
            return services.TryGet(out service);
        }
        
        bool TryGetNextInHierachy(out ServiceLocator container)
        {
            if (this == global)
            {
                container = null;
                return false;
            }
            
            container = transform.parent.OrNull()?.GetComponent<ServiceLocator>();
            return container != null;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tmpSceneGameObjects = new List<GameObject>();
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal()
        {
            var go = new GameObject(k_gloablServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }
        
        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene()
        {
            var go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif
    }
}
