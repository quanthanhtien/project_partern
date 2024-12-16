using System;
using UnityEngine;

namespace Smell
{
    public class Enemy : MonoBehaviour
    {
        public EnemyConfig Config { get; private set; }

        public void Initialize(EnemyConfig config) => Config = config;
    }
}
