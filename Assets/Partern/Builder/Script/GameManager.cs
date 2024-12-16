using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public List<EnemyFactory> enemyFactories;
    public void Start()
    {
        foreach (EnemyFactory i in enemyFactories)
        {
            Enemy enemy = i.CreateEnemy();
            Instantiate(enemy);
            Debug.Log(enemy.Name);
            Debug.Log(enemy.Health);
            Debug.Log(enemy.Speed);
            Debug.Log(enemy.Damage);
            Debug.Log(enemy.IsBoss);
            
        }
    }
}