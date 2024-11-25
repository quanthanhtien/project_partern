using UnityEngine;

public abstract class AttributeStratagy : ScriptableObject, IAttribute
{
    public abstract IAttribute AddAttribute();
    public abstract void CastSpell();
   
}

enum property
{
    health,
    mana,
    attack,
    defense
}
