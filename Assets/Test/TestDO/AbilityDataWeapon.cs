using Sirenix.OdinInspector;
using UnityEngine;

namespace n4
{
    [CreateAssetMenu(
        fileName = "AbilityDataWeapon",
        menuName = "ScriptableObjects/AbilityData",
        order = 1
    )]
    public class AbilityDataWeapon : ScriptableObject
    {
        [VerticalGroup("row1/left")]
        [VerticalGroup("row1/left")]
        public string name;

        [VerticalGroup("row1/left")]
        public string description;

        [VerticalGroup("row1/left")]
        public SpellStrategy effectSkill;

        [PreviewField(80, ObjectFieldAlignment.Right), HideLabel]
        [HorizontalGroup("row1", 80), VerticalGroup("row1/right")]
        public Sprite icon;
    }

    public abstract class SpellStrategy : ScriptableObject
    {
        public abstract void CastSpell(float dame);
    }
}
