using UnityEngine;
using UnityEngine.UI;

public class SpriteSetter : MonoBehaviour
{
    [SerializeField] private Image _image;

    public void SetSprite(Sprite sprite) => _image.sprite = sprite;
}
