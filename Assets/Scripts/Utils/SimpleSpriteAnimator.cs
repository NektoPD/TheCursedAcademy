using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class SimpleSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private float _framesPerSecond;

        [SerializeField] private bool _isLooped;
        
        private SpriteRenderer _spriteRenderer;

        private Coroutine _spriteChangeCoroutine;

        public event Action Finished;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _spriteChangeCoroutine = StartCoroutine(_isLooped ? SwitchSpritesLooped() : SwitchSpritesOnce());
        }

        private void OnDisable()
        {
            if (_spriteChangeCoroutine != null)
            {
                StopCoroutine(_spriteChangeCoroutine);
                _spriteChangeCoroutine = null;
            }
        }

        private IEnumerator SwitchSpritesLooped()
        {
            if (_spriteRenderer == null)
            {
                Debug.LogError("Sprite Renderer is null");
                yield break;
            }

            WaitForSeconds interval = new WaitForSeconds(1 / _framesPerSecond);
            
            while (true)
            {
                foreach (var t in _sprites)
                {
                    _spriteRenderer.sprite = t;
                    yield return interval;
                }
            }
        }

        private IEnumerator SwitchSpritesOnce()
        {
            if (_spriteRenderer == null)
            {
                Debug.LogError("Sprite Renderer is null");
                yield break;
            }
            
            WaitForSeconds interval = new WaitForSeconds(1 / _framesPerSecond);
            
            for (var i = 0; i < _sprites.Length; i++)
            {
                _spriteRenderer.sprite = _sprites[i];
                yield return interval;
            }
            
            Finished?.Invoke();
            gameObject.SetActive(false);
        }
    }
}