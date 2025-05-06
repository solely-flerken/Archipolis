using State;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        private VisualElement _root;
        private Label _status;
        private ProgressBar _progress;
        
        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _status = _root.Q<Label>("status");
            _progress = _root.Q<ProgressBar>("progress");

            var loadingStatus = new ViewModelLoadingStatus();
            _status.Bind(loadingStatus.CurrentStatus, nameof(Label.text));
            _progress.Bind(loadingStatus.ProgressLerp, nameof(ProgressBar.value));
        }
    }
}