using System;
using UnityEngine;
using ZLinq;
public class ExampleZlinq : MonoBehaviour
{
    public readonly int[] numbers = new[] { 1, 2, 3, 4, 5 };

    private void Start()
    {
        var result = numbers.AsValueEnumerable()
            .Where(x => x > 2)
            .Select(x => x * 2)
            .ToList();
        
        foreach (var number in result)
        {
            Debug.Log(number);
        }
        Debug.Break();
    }
}
