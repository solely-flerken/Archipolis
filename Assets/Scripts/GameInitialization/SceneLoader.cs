using System.Collections;
using Events;
using Input;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameInitialization
{
    // TODO: Maybe merge Game- and SceneLoader
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

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

        private void Start()
        {
            EventSystem.Instance.OnButtonClick += HandleButtonClick;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnButtonClick -= HandleButtonClick;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case "LoadingScene":
                    StartCoroutine(LoadMainSceneInBackground());
                    break;
                case "MainScene":
                    break;
                default:
                    // Unknown scene
                    break;
            }
        }

        private void HandleButtonClick(string buttonName)
        {
            switch (buttonName)
            {
                case "continue":
                    SceneManager.LoadScene("Scenes/LoadingScene");
                    break;
                case "newGame":
                    // Show UI for a new game
                    break;
                case "load":
                    // Show UI for loading game
                    break;
                case "exit":
                    // Exit game
                    break;
                default:
                    // Unknown button
                    break;
            }
        }

        private static IEnumerator LoadMainSceneInBackground()
        {
            var load = SceneManager.LoadSceneAsync("Scenes/MainScene", LoadSceneMode.Additive);
            load.allowSceneActivation = false;
            
            while (!load.isDone || load.progress < 0.9f)
            {
                yield return null;
            }
            
            HideAllUI();
            
            yield return GameLoader.LoadGame();

            var unload = SceneManager.UnloadSceneAsync("Scenes/LoadingScene");
            while (!unload.isDone)
            {
                yield return null;
            }
            
            // We need to update the camera after the scene has loaded
            CameraController.Camera = Camera.main;

            load.allowSceneActivation = true;
            
            ShowInitialUI();
        }
        
        private static void HideAllUI()
        {
            foreach (var userInterface in FindObjectsByType<UserInterfaceBase>(FindObjectsSortMode.None))
            {
                if (userInterface is LoadingScreen)
                {
                    continue;
                }
                
                userInterface.Hide();
            }
        }

        private static void ShowInitialUI()
        {
            foreach (var userInterface in FindObjectsByType<UserInterfaceBase>(FindObjectsSortMode.None))
            {
                if (userInterface.IsVisibleInitially)
                {
                    userInterface.Show();
                }
            }
        }
    }
}