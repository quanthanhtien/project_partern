using UnityEngine;

namespace Smell
{
    public class PlacementStrategy : ScriptableObject
    {
        public virtual Vector3 SetPosition(Vector3 origin) => origin;
    }
}
