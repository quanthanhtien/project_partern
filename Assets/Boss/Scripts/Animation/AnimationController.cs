using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AnimationController : MonoBehaviour
{
    Animator animator;
    CountdownTimer timer;

    int currentAnimation;

    float animationLength;

    [HideInInspector]
    public int locomotionClip = Animator.StringToHash("Locomotion");

    [HideInInspector]
    public int speedHash = Animator.StringToHash("Speed");

    [FormerlySerializedAs("attackClip")]
    [HideInInspector]
    public int attackClipDefault = Animator.StringToHash("Attack");
    [HideInInspector]
    public int attackClip1 = Animator.StringToHash("Attack");
    [HideInInspector]
    public int attackClip2 = Animator.StringToHash("Attack");
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        SetLocomotionClip();
        SetAttackClip();
        SetSpeedHash();
    }

    public void AnimMove()
    {
        ChangeAnimation(speedHash);
    }

    public void AnimIdle()
    {
        ChangeAnimation(locomotionClip);
    }
    public void Attack(int choose)
    {
        switch (choose)
        {
            case 1:
                ChangeAnimation(attackClipDefault);
                break;
            case 2:
                ChangeAnimation(attackClip1);
                break;
            case 3:
                ChangeAnimation(attackClip2);
                break;
        }
    }

    void Update() => timer?.Tick(Time.deltaTime);

    public void ChangeAnimation(int animation, float crossfade = 0.2f, float time = 0)
    {
        if (time > 0)
            StartCoroutine(Wait());
        else
            Validate();

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);
            Validate();
        }

        void Validate()
        {
            if (currentAnimation != animation)
            {
                currentAnimation = animation;
                animator.CrossFade(animation, crossfade);
            }
        }
    }

    protected abstract void SetLocomotionClip();
    protected abstract void SetAttackClip();
    protected abstract void SetSpeedHash();
}
