using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MVC
{
    public class AbilityButton : MonoBehaviour
    {
        public Image radialImage;
        public Image abilityIcon;
        public int index;
        public Key key;

        public event Action<int> OnButtonPressed = delegate { };

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => OnButtonPressed(index));
        }

        void Update()
        {
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                OnButtonPressed(index);
            }
        }

        public void RegisterListener(Action<int> listener)
        {
            OnButtonPressed += listener;
        }

        public void Initialize(int index, Key key)
        {
            this.key = key;
            this.index = index;
        }

        public void UpdateButtonSprite(Sprite newIcon)
        {
            abilityIcon.sprite = newIcon;
        }

        public void UpdateRadialFill(float progress)
        {
            if (radialImage)
            {
                radialImage.fillAmount = progress;
            }
        }
    }
}
