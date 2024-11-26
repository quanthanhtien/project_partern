using UnityEngine;

namespace script.Decorator
{
    public class DropZone : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (CardManager.Instance.selectedCard != null)
            {
                CardManager.Instance.selectedCard.MoveTo(transform.position);
                CardManager.Instance.selectedCard.Card.Play();
            }
        }
    }
}
