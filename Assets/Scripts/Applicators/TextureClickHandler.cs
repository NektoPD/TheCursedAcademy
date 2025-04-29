using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class TextureClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Image _image;
    
    public event Action<Image> Clicked;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(_image);
    }
}
