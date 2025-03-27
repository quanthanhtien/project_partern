using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform direct;
    public float jumpHeight = 1f;
    public List<GameObject> prefabs;
    public float duration = 0.5f;
    public int numberJump = 1;
    public int index = 0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            jump();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (index < prefabs.Count)
            {
                index++;
            }
            else
            {
                index = 0;
            }
        }
    }

    [Button]
    public void jump()
    {
        Transform direction = direct;
        GameObject obj = Instantiate(prefabs[index], spawnPoint.position, Quaternion.identity);
        obj.GetComponent<SpawnWeapon>()
            .MoveDirection(spawnPoint, direction, jumpHeight, numberJump, duration);
    }
}
