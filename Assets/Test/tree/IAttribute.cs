using System.Collections.Generic;
using UnityEngine;

public interface IAttribute
{
    public IAttribute AddAttribute();
    public void CastSpell();
}

public abstract class exp : ScriptableObject
{
    public abstract List<float> exps();
}

public abstract class level : ScriptableObject
{
    public abstract List<float> maxLevel();
}
