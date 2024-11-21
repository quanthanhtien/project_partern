using UnityEngine;

namespace script.Decorator
{
    public class DropZone : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if (CardManager.Instance.SelectedCard != null)
            {
                CardManager.Instance.SelectedCard.Card.Play();
            }
        }
    }
}