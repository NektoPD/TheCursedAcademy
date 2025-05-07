using CharacterLogic;
using UnityEditor;
using UnityEngine;
using Utils;

namespace Tutorial
{
    public class TutorialExitTrigger : MonoBehaviour
    {
        [SerializeField] private SceneAsset _scene;
        [SerializeField] private SceneChanger _changer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Character _))
                _changer.ChangeScene(_scene);
        }

        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);
    }
}