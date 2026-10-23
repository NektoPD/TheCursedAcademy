using DG.Tweening;
using TheraBytes.BetterUi;
using UnityEngine;
using UnityEngine.UI;

namespace Debuffs
{
    public sealed class CurseRevealToggleView : MonoBehaviour
    {
        [SerializeField] private BetterToggle _toggle;
        [SerializeField] private Image _togglePanel;
        [SerializeField] private Sprite _onSprite, _offSprite;

        private Tween _tween;
        private float _progress;

        private void OnEnable()
        {
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            _toggle.onValueChanged.RemoveListener(OnValueChanged);
            StopAnimation();
        }

        private void SwitchToggleSprites()
        {
            _togglePanel.sprite = _toggle.isOn ? _onSprite : _offSprite;
        }

        private void OnValueChanged(bool enabled)
        {
            SwitchToggleSprites();
        }

        private void StopAnimation()
        {
            _tween?.Kill(false);
            _tween = null;
        }
        
    }
}
