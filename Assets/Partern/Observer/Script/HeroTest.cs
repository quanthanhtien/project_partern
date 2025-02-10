using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class HeroTest : MonoBehaviour
{
    Attackable attackable;

    private void Awake()
    {
        // ServiceLocator.Global.Register<Attackable>(attackable = new Attack());
    }

    [Button]
    public void Start()
    {
        attackable.show();
    }
}

public interface Attackable
{
    void show();
}

public class Attack : Attackable
{
    public void show()
    {
        Debug.Log("Attack");
    }
}
