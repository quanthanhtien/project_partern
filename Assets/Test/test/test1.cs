using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class test1 : MonoBehaviour
{
    private void Start()
    {
        PlayerHealthBar playerHealthBar = new PlayerHealthBar();
        playerHealthBar.PlayerHealth();
    }

    [Button]
    public void playerDamage()
    {
        Player.Damage();
    }

    public class Player
    {
        public static event Action OnPlayerDamage;

        public static void Damage()
        {
            OnPlayerDamage?.Invoke();
            Debug.Log("Player Damaged1");
        }
    }

    public class PlayerHealthBar
    {
        public void PlayerHealth()
        {
            Player.OnPlayerDamage += () => Debug.Log("Player Damaged");
        }
    }
}
