using Data;
using UI.Applicators.ClickHandlers;
using UnityEngine;

namespace UI.Applicators
{
    public abstract class BaseApplicator<T> : MonoBehaviour where T : IVisualData
    {
        [SerializeField] private BaseClickHandler<T>[] _items;
        [SerializeField] private T _defaultItem;

        protected T CurrentItem;

        private void Start()
        {
            CurrentItem = _defaultItem;
        }

        protected virtual void OnEnable()
        {
            Applicate(_defaultItem);

            foreach (var item in _items)
                item.Clicked += OnClick;
        }

        protected virtual void OnDisable()
        {
            foreach (var item in _items)
                item.Clicked -= OnClick;
        }

        protected abstract void Applicate(T data);

        public void SetDefaultItem(T item) => _defaultItem = item;

        private void OnClick(T data)
        {
            CurrentItem = data;
            Applicate(data);
        }
    }
}
