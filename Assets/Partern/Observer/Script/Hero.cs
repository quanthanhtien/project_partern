using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityServiceLocator;

public class Hero : MonoBehaviour
{
    public Observer<int> Health = new Observer<int>(100);
    IAttackable attackable;

    void Start()
    {
        Health.Invoke();
    }

    private void Awake()
    {
        ServiceLocator.Global.Register<IAttackable>(attackable = new Attack1());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Health.Value += 10;
        }
    }

    [Button]
    public void Show()
    {
        ServiceLocator.For(this).Get(out attackable);
        attackable.Show();
    }
}

public interface IAttackable
{
    void Show();
}

public class Attack1 : IAttackable
{
    public void Show()
    {
        Debug.Log("Attack");
    }
}
