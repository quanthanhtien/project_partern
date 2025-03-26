using UnityEngine;

namespace n4
{
    [CreateAssetMenu(fileName = "DefaulSpell", menuName = "ScriptableObjects/Spells")]
    public class FireSpell : SpellStrategy
    {
        public GameObject vfx;

        public override void CastSpell(float dame)
        {
            Debug.Log("CastSpell");
        }
    }
}
