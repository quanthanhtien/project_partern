using UnityEngine;

namespace Smell
{
    [CreateAssetMenu(
        fileName = "RandomCirclePlacer",
        menuName = "Placement Strategy/Random Circle"
    )]
    public class RandomCirclePlacer : PlacementStrategy
    {
        public float minDistance = 2.0f;
        public float maxDistance = 10.0f;

        public override Vector3 SetPosition(Vector3 origin)
        {
            Debug.Log("RandomCirclePlacer");
            return Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
        }
    }
}
