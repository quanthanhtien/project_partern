using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    
    public float Attack()
    {
        Debug.Log("Attack");
        return 1.0f;
    }
    public float Spin()
    {
        Debug.Log("Spin");
        return 1.0f;
    }
    public float Jump()
    {
        Debug.Log("Jump"); 
        return 1.0f;
    }

    public float Idle()
    {
        Debug.Log("");
        return 0f;
    }
    
}