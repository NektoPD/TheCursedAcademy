using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Animation
{
    public sealed class UIButtonInteractionAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        private const float HoverScale = 1.06f;
        private const float PressedScale = 0.94f;
        private const float AnimationDuration = 0.12f;

        private static UIButtonInteractionAnimation _scanner;

        private Button _button;
        private Tween _scaleTween;
        private Vector3 _baseScale;
        private bool _hovered;
        private bool _pressed;
        private bool _animating;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            if (_scanner != null)
                return;

            var host = new GameObject(nameof(UIButtonInteractionAnimation));
            DontDestroyOnLoad(host);
            _scanner = host.AddComponent<UIButtonInteractionAnimation>();
            SceneManager.sceneLoaded += _scanner.OnSceneLoaded;
            _scanner.StartCoroutine(_scanner.ScanButtons());
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private IEnumerator ScanButtons()
        {
            while (true)
            {
                AttachToButtons();
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            AttachToButtons();
        }

        private static void AttachToButtons()
        {
            foreach (Button button in FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (button.GetComponent<UIButtonInteractionAnimation>() == null)
                    button.gameObject.AddComponent<UIButtonInteractionAnimation>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!CanAnimate())
                return;

            _hovered = true;
            if (!_pressed)
                Animate(HoverScale);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _hovered = false;
            if (!CanAnimate())
                ResetAnimation();
            else if (!_pressed)
                Animate(1f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !CanAnimate())
                return;

            _pressed = true;
            Animate(PressedScale);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !_pressed)
                return;

            _pressed = false;
            if (CanAnimate())
                Animate(_hovered ? HoverScale : 1f);
            else
                ResetAnimation();
        }

        private bool CanAnimate()
        {
            return _button != null && _button.IsActive() && _button.IsInteractable();
        }

        private void Animate(float scale)
        {
            if (!_animating)
            {
                _baseScale = transform.localScale;
                _animating = true;
            }

            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_baseScale * scale, AnimationDuration)
                .SetEase(scale > 1f ? Ease.OutBack : Ease.OutQuad)
                .SetUpdate(true);

            if (scale == 1f)
                _scaleTween.OnComplete(() => _animating = false);
        }

        private void OnDisable()
        {
            if (_button != null)
                ResetAnimation();
        }

        private void ResetAnimation()
        {
            _scaleTween?.Kill();
            _scaleTween = null;

            if (_animating)
                transform.localScale = _baseScale;

            _animating = false;
            _hovered = false;
            _pressed = false;
        }

        private void OnDestroy()
        {
            if (this == _scanner)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _scanner = null;
            }

            _scaleTween?.Kill();
        }
    }
}
