using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public static event Action<string> OnSpawnEnemy = delegate {  }; 
    [Button]
    public void Death()
    {
        OnSpawnEnemy?.Invoke(GetComponent<SetUniqueID>().guidString);
        Debug.Log("Enemy die");
        this.enabled = false;
    }
    
}
