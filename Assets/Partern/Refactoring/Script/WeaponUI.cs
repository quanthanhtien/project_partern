using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace script.Refactoring
{
    public class WeaponUI : MonoBehaviour
    {
        public WeaponController weaponController;

        public Transform buttonContainer;
        public Button buttonprefab;
        public List<WeaponConfig> weaponConfigs;

        private void Start()
        {
            foreach (var config in weaponConfigs)
            {
                CreateWeaponButton(config);
            }
        }

        void CreateWeaponButton(WeaponConfig config)
        {
            Button button = Instantiate(buttonprefab, buttonContainer);
            button.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = config.weaponName;
            button.onClick.AddListener(() => weaponController.EquipWeapon(config));
        }
    }
}
