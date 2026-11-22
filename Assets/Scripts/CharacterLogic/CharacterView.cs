using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using YG;

namespace CharacterLogic
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Slider _hpBar;
        [SerializeField] private Slider _levelBar;
        [SerializeField] private Image _heroImage;
        [SerializeField] private Image _abilityLevel;
        [SerializeField] private Button _abilityButton;
        [SerializeField] private Graphic _abilityDesktopElement;
        [SerializeField] private bool _forceMobileMode;

        private Canvas _hudCanvas;
        private bool _isTutorialMode;

        private Tween _abilityPulseTween;
        private Tween _levelPulseTween;
        private Tween _abilityDesktopTween;
        private Vector3 _abilityButtonInitialScale;
        private Vector3 _abilityDesktopImageInitialScale;
        private Vector3 _levelBarInitialScale;
        private bool _abilityReady;

        public event Action AbilityButtonPressed;

        private void Awake()
        {
            _hudCanvas = GetComponentInChildren<Canvas>(true);
            if (_levelBar != null)
                _levelBarInitialScale = _levelBar.transform.localScale;

            if (_abilityButton != null)
            {
                _abilityButton.onClick.AddListener(() => AbilityButtonPressed?.Invoke());
                _abilityButtonInitialScale = _abilityButton.transform.localScale;
            }

            if (_abilityDesktopElement != null)
            {
                _abilityDesktopImageInitialScale = _abilityDesktopElement.transform.localScale;
            }

            HideAbilityUI();
        }

        public void SetHudVisible(bool isVisible)
        {
            CacheHudCanvas();

            if (_hudCanvas != null)
                _hudCanvas.gameObject.SetActive(_isTutorialMode || isVisible);
        }

        public void SetTutorialMode(bool isTutorial)
        {
            _isTutorialMode = isTutorial;

            if (!isTutorial)
                return;

            CacheHudCanvas();

            if (_hudCanvas == null)
                return;

            _hudCanvas.gameObject.SetActive(true);

            foreach (Transform child in _hudCanvas.transform)
            {
                bool isAbilityButton = _abilityButton != null &&
                                       child.gameObject == _abilityButton.gameObject;

                bool isAbilityText = _abilityDesktopElement != null &&
                                     child.gameObject == _abilityDesktopElement.gameObject;

                bool isJoystick = child.GetComponent<VirtualJoystick>() != null;

                if (!isAbilityButton && !isAbilityText && !isJoystick)
                    child.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            YG2.onGetSDKData += RefreshAbilityUI;
#endif
            if (_abilityReady)
                RefreshAbilityUI();
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            YG2.onGetSDKData -= RefreshAbilityUI;
#endif
        }

        private void CacheHudCanvas()
        {
            if (_hudCanvas == null)
                _hudCanvas = GetComponentInChildren<Canvas>(true);
        }

        public void UpdateHpBar(float value, float maxHealth)
        {
            _hpBar.value = value / maxHealth;
        }

        public void UpdateAbilityLevelBar(float value, float maxValue)
        {
            _abilityLevel.fillAmount = value / maxValue;
        }

        public void UpdateLevelBar(int value, int levelRequirement)
        {
            _levelBar.value = (float)value / levelRequirement;
        }

        public void PlayLevelUpPulse()
        {
            if (_levelBar == null)
                return;

            _levelPulseTween?.Kill();
            _levelBar.transform.localScale = _levelBarInitialScale;
            _levelPulseTween = _levelBar.transform
                .DOScale(_levelBarInitialScale * 1.15f, 0.18f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(true);
        }

        public void SetHeroImage(Sprite image)
        {
            _heroImage.sprite = image;
        }

        public void ShowAbilityReady()
        {
            _abilityReady = true;
            RefreshAbilityUI();
        }

        public void HideAbilityUI()
        {
            _abilityReady = false;
            RefreshAbilityUI();
        }

        private void RefreshAbilityUI()
        {
            bool showButton = _abilityReady && TouchControls.IsEnabled(_forceMobileMode);
            UpdateAbilityElement(_abilityButton, showButton, ref _abilityPulseTween, _abilityButtonInitialScale);
            UpdateAbilityElement(_abilityDesktopElement, _abilityReady && !showButton, ref _abilityDesktopTween,
                _abilityDesktopImageInitialScale);
        }

        private void UpdateAbilityElement(Component element, bool visible, ref Tween tween, Vector3 initialScale)
        {
            if (element == null)
                return;

            element.gameObject.SetActive(visible);
            if (visible)
                PlayPulseAnimation(ref tween, element.transform, initialScale);
            else
                StopPlayingAnimation(tween, element.transform, initialScale);
        }

        private void PlayPulseAnimation(ref Tween tween, Transform transform, Vector3 initalScale)
        {
            tween ??= transform
                .DOScale(initalScale * 1.1f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false);

            tween.Play();
        }

        private void StopPlayingAnimation(Tween tween, Transform transform, Vector3 initalScale)
        {
            if (tween == null)
                return;

            tween.Pause();
            transform.transform.localScale = initalScale;
        }

        private void OnDestroy()
        {
            _abilityPulseTween?.Kill();
            _abilityDesktopTween?.Kill();
            _levelPulseTween?.Kill();
        }
    }
}