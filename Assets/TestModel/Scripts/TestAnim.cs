using System;
using Animancer;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TestAnim : MonoBehaviour
{
    [SerializeField]
    private AnimancerComponent _Animancer;

    [SerializeField]
    private AnimationClip _Idle;

    [SerializeField]
    private AnimationClip _Run;

    [SerializeField]
    private AnimationClip _Jump;

    [SerializeField]
    private AnimationClip _Attack;
    Rigidbody rb;
    private bool isGround;

    private void Start()
    {
        _Animancer.Play(_Idle);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            var state = _Animancer.Play(_Run);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(0, 4f, 0, ForceMode.Impulse);
            var state = _Animancer.Play(_Jump);
            isGround = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var state = _Animancer.Play(_Attack, 0.2f);
            state.Events.OnEnd = OnActionEnd;
        }
    }

    public void OnActionEnd()
    {
        _Animancer.Play(_Idle);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            OnActionEnd();
            isGround = true;
        }
    }
}
