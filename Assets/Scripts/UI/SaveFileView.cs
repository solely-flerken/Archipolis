using System;
using Save;
using UnityEngine.UIElements;

namespace UI
{
    [UxmlElement]
    public partial class SaveFileView : VisualElement
    {
        public Action<string> OnSaveSelection;

        public SaveFileView()
        {
            LoadSaves();
        }

        private void LoadSaves()
        {
            var saves = SaveManager.SaveSystem.GetSaveFiles();
            Clear();

            foreach (var save in saves)
            {
                var button = new Button(() => OnSaveSelection?.Invoke(save)) { text = save };
                Add(button);
            }
        }
    }
}