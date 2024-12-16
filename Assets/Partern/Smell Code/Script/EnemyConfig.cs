using UnityEngine;

namespace Smell
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        public GameObject Prefab;
        public string type;
        public int health;
        public int damage;
        public float attackRange;
    }
}