using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test2 : MonoBehaviour
{
    public Enemy enemy;

    void Start()
    {
        enemy = new Enemy();
        PlayerHealthBar playerHealthBar = new PlayerHealthBar();
        playerHealthBar.PlayerHealth(enemy);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            enemy.Damage();
        }
    }

    public class Enemy
    {
        public PlayerHealthBar Health;
        public Enemy enemy;
        public event Action OnPlayerDamage;

        public void Damage()
        {
            Health.PlayerHealth(enemy);
            Debug.Log("Player Damaged");
            OnPlayerDamage?.Invoke();
        }
    }

    public class PlayerHealthBar
    {
        int health = 100;

        public void PlayerHealth(Enemy enemy)
        {
            enemy.OnPlayerDamage += damage;
        }

        public void damage()
        {
            health -= 10;
            Debug.LogError(health);
        }
    }
}
