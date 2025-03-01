using UnityEngine;

public class HeroAnimationController : AnimationController
{
    protected override void SetLocomotionClip()
    {
        locomotionClip = Animator.StringToHash("Locomotion");
    }

    protected override void SetAttackClip()
    {
        attackClipDefault = Animator.StringToHash("Attack01_MagicWand");
    }

    protected override void SetSpeedHash()
    {
        speedHash = Animator.StringToHash("Speed");
    }
}
