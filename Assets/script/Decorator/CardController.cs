using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace script.Decorator {
    public class CardController : MonoBehaviour
    {
        [SerializeField] private Ease ease = Ease.OutBack;
        [SerializeField, Required] private CardDefinition canDefinition;
        public ICard Card { get; private set; }

        private void Awake() => Card = CardFactory.Create(canDefinition);

        private void OnMouseDown()
        {
            if (CardManager.Instance.SelectedCard == null)
            {
                CardManager.Instance.SelectedCard = this;
            }
            else
            {
                CardManager.Instance.SelectedCard = null;
            }
        }

        public void MoveTo(Vector3 position)
        {
            transform.DOMove(position, duration).SetEase(ease);
        }
        public void MoveToAndDestroy(Vector3 position)
        {
            transform.DOMove(position, duration).SetEase(ease).OnComplete(() => Destroy(gameObject));
        }
    }
}