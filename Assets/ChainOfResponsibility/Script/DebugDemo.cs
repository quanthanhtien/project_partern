using System;
using UnityEngine;

namespace COR
{
    public class DebugDemo : MonoBehaviour
    {
        [SerializeField] DebugToolkit debugToolkit;

        void Start()
        {
            debugToolkit.Log(new GeneralDebugMessage("Application started"));
            debugToolkit.Log((new StateSaveMessage("player_state", new PlayerData(100, Vector3.zero))));
            debugToolkit.Log(null);
        }
    }
    
    [Serializable]
    public struct PlayerData
    {
        public int health;
        public Vector3 position;
        
        public PlayerData(int health, Vector3 position)
        {
            this.health = health;
            this.position = position;
        }
    }
}