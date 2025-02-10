using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityServiceLocator;

public class TestService : MonoBehaviour
{
    ITestService testService;

    private void Awake()
    {
        ServiceLocator.Global.Register<ITestService>(testService = new TestService1());
    }

    [Button]
    public void Test()
    {
        ServiceLocator.For(this).Get(out testService);
        testService.Test();
    }
}

public interface ITestService
{
    void Test();
}

public class TestService1 : ITestService
{
    public void Test()
    {
        Debug.Log("Test");
    }
}
