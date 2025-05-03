using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Training
{
    public class TrainingExitTrigger : MonoBehaviour
    {
        [SerializeField] private SceneAsset _scene;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            SceneManager.LoadScene(_scene.name);
        }

        public void On() => enabled = true;

        public void Off() => enabled = false;
    }
}