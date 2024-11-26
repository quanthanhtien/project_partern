using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "expHealth", menuName = "Attribute/Health/expHealth")]
public class expHealth : exp
{
    public List<float> listExp = new();

    public override List<float> exps()
    {
        return listExp;
    }
}
