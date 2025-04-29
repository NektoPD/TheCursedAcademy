using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Applicators
{
    public abstract class BaseApplicator<T> : MonoBehaviour where T : IVisualData
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Image _image;
        [SerializeField] private BaseClickHandler<T>[] _items;
        [SerializeField] private T _defaultItem;

        protected T CurrentItem;

        protected TextMeshProUGUI Name => _name;
        protected TextMeshProUGUI Description => _description;
        protected Image MainImage => _image;

        private void Start()
        {
            CurrentItem = _defaultItem;
            Application(_defaultItem);
        }

        protected virtual void OnEnable()
        {
            foreach (var item in _items)
                item.Clicked += OnClick;
        }

        protected virtual void OnDisable()
        {
            foreach (var item in _items)
                item.Clicked -= OnClick;
        }

        protected abstract void Application(T data);

        private void OnClick(T data)
        {
            CurrentItem = data;
            Application(data);
        }
    }
}
