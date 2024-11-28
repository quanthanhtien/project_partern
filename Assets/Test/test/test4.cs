using System;
using UnityEngine;

public class test4 : MonoBehaviour
{
    public test5 n ;
    public test5 speed => n ??= GetComponent<test5>();

    private void Start()
    {
        speed.show();
    }

    public void show()
    {
        Debug.Log("show1");
    }
}   

public interface test5
{
    public void show() {}
    
}