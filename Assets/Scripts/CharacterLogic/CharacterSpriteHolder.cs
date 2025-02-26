using UnityEngine;
using UnityEngine.UI;

namespace CharacterLogic
{
    public class CharacterSpriteHolder : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void SetSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
        }

        public void FlipSprite(bool status)
        {
            _spriteRenderer.flipX = status;
        }
    }
}