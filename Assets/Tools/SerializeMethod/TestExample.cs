using System.Collections.Generic;
using UnityEngine;

public class TestExample:MonoBehaviour
{ 
    [SerializeField] SerializedCallback<string> callback1 ;
    [SerializeField] SerializedCallback<float> callback2 ;

    void Start()
    {
        var result1 = callback1.Invoke();
        var result2 = callback2.Invoke();
        Debug.Log($"Callback result: {result1}");
        Debug.Log($"Callback result: {result2}");
    }
    
    public float AddNumber(int a, int b)
    {
        return a + b;
    }
    
    public float multiplyNumber(int a, int b)
    {
        return a * b;
    }
    
     public string ConcatString(string a, string b)
     {
         return a + b;
     }
}
