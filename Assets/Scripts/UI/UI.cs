using Events;
using Input;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace UI
{
    public class UI : MonoBehaviour
    {
        public GameObject prefab;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
        
            var buttonBuild1 = root.Q<Button>("Build1");
            var buttonBuild2 = root.Q<Button>("Build2");
            var buttonBuild3 = root.Q<Button>("Build3");

            buttonBuild1.clicked += () =>
            {
                var position = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
                var newBuilding = Instantiate(prefab, position, Quaternion.identity);
                EventSystem.Instance.InvokeBuildingClick(newBuilding);
            };
            buttonBuild2.clicked += () => { };
            buttonBuild3.clicked += () => { };
        }
    }
}
