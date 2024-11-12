using UnityEngine;
[CreateAssetMenu(fileName = "ReviveSpell", menuName = "Spell/ReviveSpell")]
public class reviveSpell : SpellStrategy
{
    public override void CastSpell(Transform origin)
    {
        Debug.Log("Cast Revive Spell");
    }
}