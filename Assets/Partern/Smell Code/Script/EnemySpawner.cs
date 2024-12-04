using System.Collections.Generic;
using UnityEngine;

namespace Smell
{
    public class EnemySpawner : MonoBehaviour
    {
        public int maxEnemies = 10;
        public List<EnemyConfig> enemyConfigs;
        EnemyFactory enemyFactory = new EnemyFactory();

        private void Start()
        {
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            for (int i = 0; i < 10; i++)
            {
                Enemy enemy = enemyFactory.Create(enemyConfigs[i % enemyConfigs.Count]);
                enemy.transform.position = new Vector3(i * 2.0f, 0, 0);
            }
        }
    }

    public abstract class PlacementStrategy
    {
        public abstract void SetPosition(Enemy enemy, int index);
    }
}
