using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace script.Decorator
{
    public class CardController : MonoBehaviour
    {
        [SerializeField]
        private Ease ease = Ease.OutBack;
        public float duration = 0.5f;

        [SerializeField, Required]
        private CardDefinition canDefinition;
        public ICard Card { get; set; }

        private void Awake() => Card = CardFactory.Create(canDefinition);

        private void OnMouseDown()
        {
            if (CardManager.Instance.selectedCard == null)
            {
                CardManager.Instance.selectedCard = this;
            }
            else
            {
                CardManager.Instance.Decorate(this);
                CardManager.Instance.selectedCard = null;
            }
        }

        public void MoveTo(Vector3 position)
        {
            transform.DOMove(position, duration).SetEase(ease);
        }

        public void MoveToAndDestroy(Vector3 position)
        {
            transform
                .DOMove(position, duration)
                .SetEase(ease)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}
