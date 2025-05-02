using Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Applicators
{
    public class CharacterApplicator : BaseApplicator<CharacterVisualData>
    {
        private const string Key = "CharacterId";

        [SerializeField] private Image _item;
        [SerializeField] private Button _play;
        [SerializeField] private SceneAsset _scene;

        protected override void OnEnable()
        {
            base.OnEnable();
            _play.onClick.AddListener(OnPlayClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _play.onClick.RemoveListener(OnPlayClick);
        }

        protected override void Applicate(CharacterVisualData data)
        {
            Name.text = data.Name;
            Description.text = data.Description;
            MainImage.sprite = data.Sprite;
            _item.sprite = data.Data.StartItem.Data.ItemIcon;
        }

        private void OnPlayClick()
        {
            PlayerPrefs.SetInt(Key, (int)CurrentItem.Data.Type);
            SceneManager.LoadScene(_scene.name);
        }
    }
}