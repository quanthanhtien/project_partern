using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Attribute/Health/Health")]
public class Health : AttributeStratagy
{
    public exp expHealth;
    public level levelHealth;
    public AttributeStratagy inputAttribute;
    public override IAttribute AddAttribute()
    {
        return inputAttribute;
    }

    public override void CastSpell()
    {
        Debug.Log("Cast Health Spell");
    }
}