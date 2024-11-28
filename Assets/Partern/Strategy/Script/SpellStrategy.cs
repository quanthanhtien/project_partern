using System;
using UnityEngine;

public abstract class SpellStrategy : ScriptableObject
{
    public abstract void CastSpell(Transform origin);
    
}