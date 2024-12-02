using UnityEngine;

public class Flyweight :MonoBehaviour
{
    public FlyweightSetting settings;
}
public abstract class FlyweightSetting : ScriptableObject
{
    public GameObject prefab;
    public float despawnDelay;
    public float speed;
    
    public Flyweight CreateFlyweight()
    {
        var go = Instantiate(prefab);
        go.SetActive(false);
        go.name = prefab.name;
        var flyweight = go.AddComponent<Flyweight>();
        flyweight.settings = this;
        return flyweight;
    }
}
