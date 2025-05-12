using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    [RequireComponent(typeof(FadeController))]
    public class SceneChanger : MonoBehaviour
    {
        private FadeController _fade;
        private AsyncOperation _loadingSceneOperation;

        private void Awake()
        {
            _fade = GetComponent<FadeController>();
        }

        public void ChangeScene(int idScene)
        {
            Time.timeScale = 1f;

            _loadingSceneOperation = SceneManager.LoadSceneAsync(idScene);
            _loadingSceneOperation.allowSceneActivation = false;

            _fade.FadeIn();
        }

        private void ChangeToggle() => _loadingSceneOperation.allowSceneActivation = true;
    }
}