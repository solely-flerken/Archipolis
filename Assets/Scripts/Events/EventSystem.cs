using System;
using Buildings;
using UnityEngine;
using Utils;

namespace Events
{
    public class EventSystem : MonoBehaviour
    {
        public static EventSystem Instance { get; private set; }

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

        public event Action OnClick;

        public event Action<GameObject> OnClickableClick;
        public event Action<GameObject> OnBuildingSelected;
        public event Action<GameObject> OnBuildingPlaced;

        public void InvokeClick()
        {
            OnClick?.Invoke();
            
            var objectUnderMouse = MouseUtils.GetObjectUnderMouse(Camera.main);
                
            if (objectUnderMouse is null) return;
            if (!objectUnderMouse.TryGetComponent<IClickable>(out var component)) return;

            component.OnClick(objectUnderMouse);
            
            // Worse performance since every component listening to OnClickableClick does checks.
            // Only use for UI etc.
            InvokeClickableClick(objectUnderMouse);

            if (!objectUnderMouse.TryGetComponent<Building>(out _))
            {
                InvokeBuildingSelected(objectUnderMouse);
            }
        }

        private void InvokeClickableClick(GameObject obj)
        {
            OnClickableClick?.Invoke(obj);
        }

        private void InvokeBuildingSelected(GameObject obj)
        {
            OnBuildingSelected?.Invoke(obj);
        }

        public void InvokeBuildingPlaced(GameObject obj)
        {
            OnBuildingPlaced?.Invoke(obj);
        }
    }
}