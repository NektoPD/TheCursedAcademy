using Data;
using System;
using UnityEngine.EventSystems;
using UnityEngine;

namespace UI.Applicators.ClickHandlers
{
    public abstract class BaseClickHandler<T> : MonoBehaviour, IPointerClickHandler where T : IVisualData
    {
        [SerializeField] private T _data;

        protected T Data => _data;

        public event Action<T> Clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(_data);
        }

        public void SetData(T data) => _data = data;
    }
}