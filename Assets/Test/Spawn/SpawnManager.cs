using System;
using UnityEngine;
using System.Collections.Generic;
public class SpawnManager :MonoBehaviour {
    public List<GameObject> enemies;
    public GameObject enemyPrefab;
    private void OnEnable()
    {
        Enemies.OnSpawnEnemy += SpawnEnemy;
    }

    private void OnDisable()
    {
        Enemies.OnSpawnEnemy -= SpawnEnemy;
    }

    public void SpawnEnemy(string id)
    {
        foreach (var enemy in enemies)
        {
            if (id == enemy.GetComponent<SetUniqueID>().guidString || enemy == null)
            {
                enemies.Remove(enemy);
                var newEnemy = Instantiate(enemyPrefab, enemy.transform.position, enemy.transform.rotation);   
                enemies.Add(newEnemy);
            }
        }
    }
}
