using System;
using UnityEngine;

public class SubCollectible : MonoBehaviour
{
    [SerializeField] int points = 10;
    
    public static event Action<int> OnSubCollected; 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collected");
            OnSubCollected?.Invoke(points);
            gameObject.SetActive(false);
        }
    }
}