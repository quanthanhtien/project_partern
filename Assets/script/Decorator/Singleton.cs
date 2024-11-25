using UnityEngine;

namespace script.Decorator
{
    public class Singleton<T> : MonoBehaviour
        where T : Component
    {
        private static T _instance;
        public static bool HasInstance => _instance != null;
        public static T TryGetInstance => HasInstance ? _instance : null;
        public static T Current => _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name;
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected void Awake()
        {
            InitializeSingleton();
        }

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            _instance = this as T;
        }
    }
}
