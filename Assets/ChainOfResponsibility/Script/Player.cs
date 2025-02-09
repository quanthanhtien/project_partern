using System.Collections;
using System.Collections.Generic;
using script.Decorator;
using UnityEngine;

namespace COR
{
    public class Player :Singleton<Player>
    {
        public Observer<int> Health = new Observer<int>(default);

        void Start()
        {
            Health.Invoke();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Health.Value += 10;
            }
        }
    }
    
}