using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smell
{
    public class EnemyFactory : IEnemyFactory
    {
        public Enemy Create(EnemyConfig config)
        {
            GameObject enemy = Object.Instantiate(config.Prefab);
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            enemyComponent.Initialize(config);
            return enemyComponent;
        }
    }
}
