using CharacterLogic;
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
            if(collision.TryGetComponent(out Character _))
                SceneManager.LoadScene(_scene.name);
        }

        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);
    }
}