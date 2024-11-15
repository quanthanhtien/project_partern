using System;
using UnityEngine;

public class test1 : MonoBehaviour
{
    private void Start()
    {
        Player player = new Player();
        PlayerHealthBar playerHealthBar = new PlayerHealthBar();
        playerHealthBar.PlayerHealth();
    }
    
    public class Player
    {
        public event Action OnPlayerDamage;
        public void Damage()
        {
            OnPlayerDamage?.Invoke();
            Debug.Log("Player Damaged1");
        }
    }

    public class PlayerHealthBar
    {
        public Player player;
        public void PlayerHealth()
        {
            player.OnPlayerDamage += () => Debug.Log("Player Damaged");
        }
    }
}