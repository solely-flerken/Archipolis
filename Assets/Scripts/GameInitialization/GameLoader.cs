using System.Collections;
using Buildings;
using GameResources;
using Hex;
using Save;
using UnityEngine;

namespace GameInitialization
{
    public class GameLoader : MonoBehaviour
    {
        public static GameLoader Instance { get; private set; }
        
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
                return;
            }
        }

        // TODO: Refactor
        public static IEnumerator LoadGame()
        {
            LoadingProgressManager.LoadingMessage = "Loading save...";
            var saveData = SaveManager.Instance.LoadLatestGame();
            LoadingProgressManager.Instance.UpdateProgress(20f);
            yield return new WaitForSeconds(1f);;
            
            LoadingProgressManager.LoadingMessage = "Loading resources...";
            yield return HexMapManager.Instance.Initialize(saveData);
            LoadingProgressManager.Instance.UpdateProgress(40f);
            yield return new WaitForSeconds(1f);
            
            LoadingProgressManager.LoadingMessage = "Loading buildings...";
            yield return BuildingManager.Instance.Initialize(saveData);
            LoadingProgressManager.Instance.UpdateProgress(60f);
            yield return new WaitForSeconds(1f);
            
            LoadingProgressManager.LoadingMessage = "Loading resources...";
            yield return ResourceManager.Initialize(saveData);
            LoadingProgressManager.Instance.UpdateProgress(80f);
            yield return new WaitForSeconds(1f);
            
            LoadingProgressManager.LoadingMessage = "Finalizing...";
            LoadingProgressManager.Instance.UpdateProgress(100f);
            yield return new WaitForSeconds(1f);
        }
    }
}