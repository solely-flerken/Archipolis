using Events;
using UnityEngine;

namespace State
{
    public class StateManager : MonoBehaviour
    {
        public static StateManager Instance { get; private set; }

        // States
        public bool IsBulldoze {get; private set;}
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void SetBulldoze(bool isActive)
        {
            IsBulldoze = isActive;
            EventSystem.Instance.InvokeBulldoze(isActive);
        }
        
        public void ToggleBulldoze()
        {
            SetBulldoze(!IsBulldoze);
        }
    }
}