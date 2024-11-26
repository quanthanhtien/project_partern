using UnityEngine;

[CreateAssetMenu(fileName = "FireballSpell", menuName = "Spell/FireballSpell")]
public class fireballSpell : SpellStrategy
{
    public override void CastSpell(Transform origin)
    {
        Debug.Log("Cast Fireball Spell");
    }
}