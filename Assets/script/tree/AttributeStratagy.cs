using UnityEngine;

public abstract class AttributeStratagy : ScriptableObject, IAttribute
{
    public abstract IAttribute AddAttribute();
    public abstract void CastSpell();
    public exp exp;
    public level level;
}
