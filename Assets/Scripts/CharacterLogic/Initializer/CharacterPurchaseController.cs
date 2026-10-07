using System;
using System.Linq;
using CharacterLogic.Data;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace CharacterLogic.Initializer
{
    public class CharacterPurchaseController : MonoBehaviour
    {
        [SerializeField] private Image[] _lockedImages;
        [SerializeField] private TMP_Text[] _prices;
        [SerializeField] private CharacterVisualData[] _characterVisualDatas;

        public CharacterUnlockData CharacterUnlockData { get; private set; }

        private void Awake()
        {
            CharacterUnlockData = YandexGame.savesData.CharacterUnlockDatas ??
                                  throw new ArgumentNullException(nameof(CharacterUnlockData));
        }

        private void Start()
        {
            SetupCharacterPurchaseVisuals();
        }

        public bool IsCharacterAvailable(CharacterData.CharacterType characterType)
        {
            return CharacterUnlockData.Data.FirstOrDefault(d => d.Key == characterType).Value;
        }

        public bool TryUnlockCharacter(CharacterData.CharacterType characterType)
        {
            if (!CharacterUnlockData.Data.ContainsKey(characterType)) return false;
            CharacterUnlockData.Data[characterType] = true;
            YandexGame.SaveProgress();
            SetupCharacterPurchaseVisuals();
            return true;
        }

        private void SetupCharacterPurchaseVisuals()
        {
            for (var i = 0; i < CharacterUnlockData.Data.Count; i++)
            {
                _lockedImages[i].gameObject.SetActive(!CharacterUnlockData.Data.ElementAt(i).Value);
                _prices[i].text = _characterVisualDatas[i].Data.UnlockPrice.ToString();
            }
        }
    }
}