using Sirenix.OdinInspector;
using UnityEngine;

namespace n4
{
    [CreateAssetMenu(fileName = "DataWeapon", menuName = "DataWeapon/AbilityData", order = 1)]
    public class DataWeapon : ScriptableObject
    {
        [VerticalGroup("row1/left")]
        public string name;

        [VerticalGroup("row1/left")]
        public string description;

        [VerticalGroup("row1/left")]
        public SpellStrategy effectSkill;

        [VerticalGroup("row1/left")]
        public GameObject vfx;

        [PreviewField(100, ObjectFieldAlignment.Right), HideLabel]
        [HorizontalGroup("row1", 100), VerticalGroup("row1/right")]
        public Sprite icon;

        [VerticalGroup("row1/left")]
        public float damage;

        [VerticalGroup("row1/left")]
        public float lifeTime;

        [VerticalGroup("row1/left")]
        public float coolDown;
    }

    public enum TypeSkill
    {
        CreateDamage,
        CrowdControl,
        AreaDamage,
    }

    public abstract class SpellStrategy : ScriptableObject
    {
        public abstract void CashSpell(Vector3 pos, GameObject area, GameObject target);
    }

    public abstract class EffectStrategy : ScriptableObject
    {
        public abstract void EffectSpell(Vector3 pos, GameObject area, GameObject target);
    }
}
