using NUnit.Framework;
using UnityEngine;

public abstract class EnemyFactory : ScriptableObject
{
    public abstract Enemy CreateEnemy();
}
[CreateAssetMenu(fileName = "EnemyFactory", menuName = "Factory/EnemyFactory")]
public class CreateEnemyFactory : EnemyFactory
{
    public string Name;
    public int Health;
    public float Speed;
    public int Damage;
    public bool IsBoss;
    public override Enemy CreateEnemy()
    {
        Enemy enemy = new Builder()
            .withName(Name)
            .withHealth(Health)
            .withSpeed(Speed)
            .withDamage(Damage)
            .withIsBoss(IsBoss).build();
        return enemy;
    }
}