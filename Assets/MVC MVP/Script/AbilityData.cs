using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "ScriptableObjects/AbilityData", order = 1)]
public class AbilityData : ScriptableObject
{
    public AnimationClip animationClip;
    public int animationHash;
    public float duration;
    public Sprite icon;
    public string fullName;

    void OnValidate()
    {
        animationHash = Animator.StringToHash(animationClip.name);
    }
}
