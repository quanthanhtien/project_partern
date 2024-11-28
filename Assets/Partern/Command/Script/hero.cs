using UnityEngine;

public class hero : MonoBehaviour, IEntity
{
    AnimationManager animationManager;
    public AnimationManager Animations => animationManager ??= GetComponent<AnimationManager>();
    public void Attack()
    {
        Debug.Log("Attack");
    }

    public void Spin()
    {
        Debug.Log("Spin");
    }

    public void Jump()
    {
        Debug.Log("Jump");
    }
}