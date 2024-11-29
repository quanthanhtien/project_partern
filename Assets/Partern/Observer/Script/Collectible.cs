using UnityEngine;
using System;
public class Collectible : MonoBehaviour
{
    [SerializeField] int points = 10;


    public static event Action<int> OnAnyCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collected");
            OnAnyCollected?.Invoke(points);
            gameObject.SetActive(false);
        }
    }
}