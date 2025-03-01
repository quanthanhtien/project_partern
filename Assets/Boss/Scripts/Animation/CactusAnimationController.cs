using UnityEngine;

public class CactusAnimationController : AnimationController
{
    protected override void SetLocomotionClip()
    {
        locomotionClip = Animator.StringToHash("Idel");
    }

    protected override void SetAttackClip()
    {
        attackClipDefault = Animator.StringToHash("Atack_3");
    }

    protected override void SetSpeedHash()
    {
        speedHash = Animator.StringToHash("Run");
    }
}
