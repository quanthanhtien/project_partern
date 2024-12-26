using UnityEngine;

namespace Platformer
{
    public interface IState
    {
        void OnEnter();
        void OnUpdate();
        void OnFixedUpdate();
        void OnExit();
    }

    public class PlayerController : MonoBehaviour
    {
    
    }
}

