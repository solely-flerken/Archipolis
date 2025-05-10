using GameInitialization;
using UnityEngine.UIElements;

namespace UI
{
    public class LoadingScreen : UserInterfaceBase
    {
        private Label _status;
        private ProgressBar _progress;
        
        private void Start()
        {
            IsVisibleInitially = true;
            
            Root = GetComponent<UIDocument>().rootVisualElement;
            _status = Root.Q<Label>("status");
            _progress = Root.Q<ProgressBar>("progress");

            var loadingStatus = new ViewModelLoadingStatus();
            _status.Bind(loadingStatus.CurrentStatus, nameof(Label.text));
            _progress.Bind(loadingStatus.ProgressLerp, nameof(ProgressBar.value));
        }
    }
}