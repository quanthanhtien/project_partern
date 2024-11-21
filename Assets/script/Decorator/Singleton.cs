using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static bool HasInstance => instance != null;
    public static T TryGetInstance => HasInstance ? instance : null;
    public static T Current => instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
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
        instance = this as T;
    }
}