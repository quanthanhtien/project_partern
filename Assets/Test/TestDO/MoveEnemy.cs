using System;
using UnityEngine;

public class MoveEnemy : MonoBehaviour
{
    public float speed = 10f;

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        transform.position += new Vector3(x, 0, z) * speed * Time.deltaTime;
        transform.LookAt(transform.position + new Vector3(x, 0, z));
    }
}
