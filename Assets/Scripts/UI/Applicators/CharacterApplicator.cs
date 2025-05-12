using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Applicators
{
    public class CharacterApplicator : BaseApplicator<CharacterVisualData>
    {
        private const string Key = "CharacterId";

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;
        [SerializeField] private Image _item;
        [SerializeField] private Button _play;
        [SerializeField] private int _gameIdScene;
        [SerializeField] private SceneChanger _changer;

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
            _name.text = data.Name;
            _description.text = data.Description;
            _image.sprite = data.Sprite;
            _item.sprite = data.Data.StartItem.Data.ItemIcon;
        }

        private void OnPlayClick()
        {
            PlayerPrefs.SetInt(Key, (int)CurrentItem.Data.Type);
            _changer.ChangeScene(_gameIdScene);
        }
    }
}