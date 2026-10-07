using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Untils
{
    public class SpriteFader : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _area;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private float _fadeDuration;

        private List<SpriteRenderer> _sprites;
        private float[] _originalAlphas;
        private Coroutine _fadeRoutine;

        public void FadeOut()
        {
            EnsureCache();

            foreach (var sprite in _sprites)
                sprite.gameObject.SetActive(true);

            if (_fadeRoutine != null) 
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeTo(0f, disableAfter: true));
        }

        public void FadeIn()
        {
            if (_sprites == null) 
                return;

            foreach (var sprite in _sprites)
                sprite.gameObject.SetActive(true);

            if (_fadeRoutine != null) 
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeToOriginals());
        }

        private void EnsureCache()
        {
            if (_sprites != null) 
                return;

            _sprites = FindAllSpritestInArea();
            _originalAlphas = new float[_sprites.Count];

            for (int i = 0; i < _sprites.Count; i++)
                _originalAlphas[i] = _sprites[i] != null ? _sprites[i].color.a : 1f;
        }

        private IEnumerator FadeToOriginals()
        {
            yield return FadeRoutine(_originalAlphas);
        }

        private IEnumerator FadeTo(float target, bool disableAfter)
        {
            var targets = new float[_sprites.Count];

            for (int i = 0; i < targets.Length; i++) targets[i] = target;

            yield return FadeRoutine(targets);

            if (disableAfter)
                foreach (var sprite in _sprites)
                    sprite.gameObject.SetActive(false);
        }

        private IEnumerator FadeRoutine(float[] targets)
        {
            if (_sprites == null || _sprites.Count == 0) 
                yield break;

            var startAlphas = new float[_sprites.Count];

            for (int i = 0; i < _sprites.Count; i++)
                startAlphas[i] = _sprites[i] != null ? _sprites[i].color.a : 0f;

            float time = 0f;

            while (time < _fadeDuration)
            {
                time += Time.unscaledDeltaTime;
                float lerp = Mathf.Clamp01(time / _fadeDuration);

                for (int i = 0; i < _sprites.Count; i++)
                {
                    SpriteRenderer sprite = _sprites[i];

                    Color color = sprite.color;
                    color.a = Mathf.Lerp(startAlphas[i], targets[i], lerp);
                    sprite.color = color;
                }
                yield return null;
            }
        }

        private List<SpriteRenderer> FindAllSpritestInArea()
        {
            var result = new List<SpriteRenderer>();

            var filter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = _layer,
                useTriggers = true
            };

            var hits = new List<Collider2D>();
            _area.OverlapCollider(filter, hits);

            foreach (var hit in hits)
            {
                if (hit == _area)
                    continue;

                if (hit.TryGetComponent(out SpriteRenderer sprite))
                    result.Add(sprite);
            }

            return result;
        }
    }
}