using UnityEngine;

public class Builder 
{
    private string name = "Enemy";
    private int health = 10;
    private float speed = 2f;
    private int damage = 10;
    private bool IsBoss = true;
    
    public Builder withName(string name)
    {
        this.name = name;
        return this;
    }
    
    public Builder withHealth(int health)
    {
        this.health = health;
        return this;
    }
    
    public Builder withSpeed(float speed)
    {
        this.speed = speed;
        return this;
    }
    
    public Builder withDamage(int damage)
    {
        this.damage = damage;
        return this;
    }
    
    public Builder withIsBoss(bool IsBoss)
    {
        this.IsBoss = IsBoss;
        return this;
    }
    public Enemy build()
    {
        
        Enemy enemy = new GameObject("Enemy").AddComponent<Enemy>();
        enemy.Name = name;
        enemy.Health = health;
        enemy.Speed = speed;
        enemy.Damage = damage;
        enemy.IsBoss = IsBoss;
        return enemy;
    }
}