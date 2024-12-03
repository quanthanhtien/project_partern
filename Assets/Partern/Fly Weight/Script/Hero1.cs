using System;
using UnityEngine;
using  FlyWeight;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class Hero1 : MonoBehaviour
{
    public List<ProjectileSettings> projectiles;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var flyweight = FlyweightFactory.Spawn(projectiles[0]);
            Debug.Log("Projectile 1");
            flyweight.transform.position += new Vector3(-1f,1f,-1f);
            flyweight.transform.rotation = transform.rotation;
        }
        if (Input.GetMouseButtonDown(0))
        {
            var flyweight = FlyweightFactory.Spawn(projectiles[1]);
            Debug.Log("Projectile 2");
            flyweight.transform.position += new Vector3(-1f,1f,-1f);
            flyweight.transform.rotation = transform.rotation;
        }
    }
}