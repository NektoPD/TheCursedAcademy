using UnityEngine;

namespace CharacterLogic
{
    public class CharacterSpriteHolder : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public void FlipSprite(bool status)
        {
            _spriteRenderer.flipX = status;
        }
    }
}