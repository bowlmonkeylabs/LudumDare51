using UnityEngine;
using UnityEngine.SceneManagement;

namespace BML.Scripts
{
    public class AdditiveSceneLoader : MonoBehaviour
    {
        [SerializeField] private string _sceneName;

        public void LoadScene()
        {
            SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
        }

        public void UnloadScene()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}