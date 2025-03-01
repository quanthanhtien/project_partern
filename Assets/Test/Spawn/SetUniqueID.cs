using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class SetUniqueID : MonoBehaviour
{
    public string guidString;

    private void Start()
    {
        CreateUniqueID();
    }

    [Button]
    public void CreateUniqueID()
    {
        this.guidString = Guid.NewGuid().ToString();
    }
}
