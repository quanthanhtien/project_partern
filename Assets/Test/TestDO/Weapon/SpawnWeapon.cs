using DG.Tweening;
using UnityEngine;

public class SpawnWeapon : MonoBehaviour
{
    public void MoveDirection(
        Transform spawnPoint,
        Transform direction,
        float jumpHeight,
        int numberJump,
        float duration
    )
    {
        Vector3 midPoint = (spawnPoint.position + direction.position) / 2;
        midPoint.y += jumpHeight;
        Vector3 midPoint1 = (midPoint + direction.position) / 2;
        midPoint1.y += jumpHeight * 0.25f;
        transform
            .DOPath(
                new[] { spawnPoint.position, midPoint, midPoint1, direction.position },
                duration,
                PathType.CatmullRom
            )
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });

        transform
            .DORotate(new Vector3(360, 360, 0), duration, RotateMode.FastBeyond360)
            .SetLoops(-1);
    }
}
