using UnityEngine;

namespace n4
{
    [CreateAssetMenu(fileName = "PoisonSpell", menuName = "Spells/PoisonSpell")]
    public class PoisonSpell : SpellStrategy
    {
        public override void CashSpell(Vector3 pos, GameObject area, GameObject target)
        {
            var obj = Instantiate(area, pos, Quaternion.identity);
            obj.transform.SetParent(target.transform);
        }
    }
}
