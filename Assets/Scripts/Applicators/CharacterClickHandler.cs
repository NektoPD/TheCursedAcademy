using CharacterLogic.Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Applicators
{
    public class CharacterClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private CharacterData _characterData;

        public event Action<CharacterData> Clicked;

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(_characterData);
        }
    }
}
