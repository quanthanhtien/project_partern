using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Observer<T>
{
    [SerializeField] T value;
    [SerializeField] UnityEvent<T> onValueChanged;

    public T Value
    {
        get => value;
        set => Set(value);
    }

    public Observer(T value, UnityAction<T> callBack = null)
    {
        this.value = value;
        onValueChanged = new UnityEvent<T>();
        if (callBack == null) onValueChanged.AddListener(callBack);
    }
    
    public void Set(T value)
    {
        if (Equals(this.value, value)) return;
        this.value = value;
        Invoke();
    }

    public void Invoke()
    {
        Debug.Log($"Invoking {onValueChanged.GetPersistentEventCount()} listeners");
        onValueChanged.Invoke(value);
        
    }
    
    public void AddListener(UnityAction<T> callBack)
    {
        if (callBack == null) return;
        if (onValueChanged == null) onValueChanged = new UnityEvent<T>();
        
        onValueChanged.AddListener(callBack);
    }
    
    public void RemoveListener(UnityAction<T> callBack)
    {
        if (callBack == null) return;
        if (onValueChanged == null) return;
        
        onValueChanged.RemoveListener(callBack);
    }
    
    public void RemoveAllListeners()
    {
        if (onValueChanged == null) return;
        onValueChanged.RemoveAllListeners();
    }
    
    public void Dispose()
    {
        RemoveAllListeners();
        onValueChanged = null;
        value = default;
    }
    
    
}