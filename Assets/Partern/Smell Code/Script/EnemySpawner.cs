using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Smell
{
    public class EnemySpawner : MonoBehaviour
    {
        public int maxEnemies = 10;
        public List<EnemyConfig> enemyConfigs;
        EnemyFactory enemyFactory = new();

        [Required]
        public PlacementStrategy placementStrategy;

        private void Start()
        {
            SpawnEnemy();
        }

        public void SpawnEnemy()
        {
            for (int i = 0; i < maxEnemies; i++)
            {
                Enemy enemy = enemyFactory.Create(enemyConfigs[i % enemyConfigs.Count]);
                enemy.transform.position = placementStrategy.SetPosition(transform.position);
            }
        }
    }
}
