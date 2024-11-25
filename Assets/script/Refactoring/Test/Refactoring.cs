using System;
using UnityEngine;

public class Refactoring : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Test>() != null)
        {
            other.gameObject.GetComponent<Test>().Show();
        }
    }
}