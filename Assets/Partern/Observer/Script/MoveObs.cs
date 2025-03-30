using UnityEngine;

public class MoveObs : MonoBehaviour
{
   public void Move(int speed)
   {
      transform.position += Vector3.forward * speed * Time.deltaTime;
   }
}
