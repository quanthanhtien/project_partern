using UnityEngine;
[CreateAssetMenu(fileName = "SheildSpell", menuName = "Spell/SheildSpell")]
public class sheildSpell : SpellStrategy
{
    public override void CastSpell(Transform origin)
    {
        Debug.Log("Cast Sheild Spell");
    }
    
}