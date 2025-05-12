using CharacterLogic;
using UnityEngine;
using Utils;

namespace Tutorial
{
    public class TutorialExitTrigger : MonoBehaviour
    {
        [SerializeField] private int _menuIdScene;
        [SerializeField] private SceneChanger _changer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Character _))
                _changer.ChangeScene(_menuIdScene);
        }

        public void On() => gameObject.SetActive(true);

        public void Off() => gameObject.SetActive(false);
    }
}