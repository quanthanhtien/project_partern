using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelHealth", menuName = "Attribute/Health/levelHealth")]
public class levelHealth : level
{
    public List<float> listLevel = new List<float> { };

    public override List<float> maxLevel()
    {
        return listLevel;
    }
}