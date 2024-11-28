using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class test3 : MonoBehaviour
{
    private float speed = 5f;
    private Rigidbody rb;
    float rotationSpeed = 20f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float getX = Input.GetAxisRaw("Horizontal");
        float getZ = Input.GetAxisRaw("Vertical");
        rb.linearVelocity = new Vector3(getX * speed, rb.linearVelocity.y, getZ * speed);
        Vector3 toRotation = Vector3.RotateTowards(
            transform.forward,
            new Vector3(getX, 0, getZ),
            rotationSpeed * Time.deltaTime,
            0.0f
        );
        transform.rotation = Quaternion.LookRotation(toRotation);
    }
}
