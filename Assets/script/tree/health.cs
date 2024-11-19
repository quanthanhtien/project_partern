using UnityEngine;

[CreateAssetMenu(fileName = "Health", menuName = "Attribute/Health/Health")]
public class Health : AttributeStratagy
{
    public exp exp;
    public level level;

    public override IAttribute AddAttribute()
    {
        return this;
    }

    public override void CastSpell()
    {
        Debug.Log("Cast Health Spell");
    }
}