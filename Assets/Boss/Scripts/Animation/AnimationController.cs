using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class AnimationController : MonoBehaviour
{
    const float k_crossfadeDuration = 0.1f;

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

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        SetLocomotionClip();
        SetAttackClip();
        SetSpeedHash();
    }

    public void SetSpeed(float speed, Sensor sensor)
    {
        if (sensor.IsTargetInRange == false)
        {
            if (speed != 0)
            {
                ChangeAnimation(speedHash);
            }
            else
            {
                ChangeAnimation(locomotionClip);
            }
        }
    }

    public void Attack(int choose)
    {
        switch (choose)
        {
            case 1:
                ChangeAnimation(attackClipDefault);
                break;
            case 2:
                ChangeAnimation(attackClipDefault);
                break;
        }
    }

    public void PlayAnimation2(int clipHash1, int clipHash2)
    {
        PlayAnimationUsingTimer(clipHash1, clipHash2);
    }

    void Update() => timer?.Tick(Time.deltaTime);

    void PlayAnimationUsingTimer(int clipHash, int clipHash2)
    {
        timer = new CountdownTimer(GetAnimationLength(clipHash));
        timer.OnTimerStart += () => ChangeAnimation(clipHash);
        timer.OnTimerStop += () => ChangeAnimation(locomotionClip);
        timer.Start();
    }

    public float GetAnimationLength(int hash)
    {
        if (animationLength > 0)
            return animationLength;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (Animator.StringToHash(clip.name) == hash)
            {
                animationLength = clip.length;
                return clip.length;
            }
        }

        return -1f;
    }

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
