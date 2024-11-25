using System;
using UnityEngine;

public class tes : MonoBehaviour
{
    public delegate void showlog(player player);
    private void Start()
    {
        showlog log = null;
        player player = new player();
        log += playerDamage;
        log += playerHeal;
        log?.Invoke(player);
    }

    public class player
    
    {
        public void Damage()
        {
            Debug.Log("Player Damaged");
        }
    }
    public void playerDamage(player player)
    {
        player.Damage();
    }
    public void playerHeal(player player)
    {
        player.Damage();
    }
}