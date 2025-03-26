using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform direction;
    public float jumpHeight = 1f;
    public GameObject prefab;
    public float duration = 0.5f;
    public int numberJump = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { }
    }

    [Button]
    public void jump()
    {
        GameObject obj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        // obj.transform.position += direction * speed * Time.deltaTime;
        obj.transform.DOLocalJump(direction.position, jumpHeight, numberJump, duration, true)
            .SetEase(Ease.OutQuad);
        ;
    }
}
