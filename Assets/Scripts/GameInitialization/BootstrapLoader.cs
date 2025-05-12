using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameInitialization
{
    public class BootstrapLoader : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("Scenes/Bootstrap", LoadSceneMode.Additive);
        }
    }
}