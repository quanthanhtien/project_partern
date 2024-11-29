using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Start()
    {
        Enemy enemy = new Builder()
            .withName("Enemy")
            .withHealth(100)
            .withSpeed(1.0f)
            .withDamage(10)
            .withIsBoss(false).build();

        Instantiate(enemy);
    }
}