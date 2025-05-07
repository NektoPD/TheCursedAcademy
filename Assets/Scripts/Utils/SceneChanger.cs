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
        private SceneAsset _scene;

        private void Awake()
        {
            _fade = GetComponent<FadeController>();
        }

        public void ChangeScene(SceneAsset asset)
        {
            _scene = asset;
            _fade.FadeIn();
        }

        private void ChangeToggle() => SceneManager.LoadScene(_scene.name);
    }
}