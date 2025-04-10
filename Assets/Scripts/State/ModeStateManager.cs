using Events;
using UnityEngine;

namespace State
{
    public enum Mode
    {
        Idle,
        Bulldozing,
        Building,
        Placing,
    }
    
    public class ModeStateManager : MonoBehaviour
    {
        public static ModeStateManager Instance { get; private set; }

        // States
        public Mode ModeState { get; private set; } = Mode.Idle;
        
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

        public void SetMode(Mode newMode)
        {
            ModeState = newMode;
            EventSystem.Instance.InvokeModeChanged(ModeState);
        }
    }
}