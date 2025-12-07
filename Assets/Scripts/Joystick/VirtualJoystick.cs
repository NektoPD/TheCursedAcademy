using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private float _handleRange = 1f;
    [SerializeField] private float _deadZone = 0.1f;

    public Vector2 Direction { get; private set; }

    private Canvas _canvas;
    private Camera _uiCamera;
    private Vector2 _input = Vector2.zero;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();

        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            _uiCamera = _canvas.worldCamera;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _background,
                eventData.position,
                _uiCamera,
                out localPoint))
        {
            return;
        }

        Vector2 radius = _background.sizeDelta / 2f;
        _input = new Vector2(localPoint.x / radius.x, localPoint.y / radius.y);

        _input = Vector2.ClampMagnitude(_input, 1f);

        if (_input.magnitude < _deadZone)
        {
            _input = Vector2.zero;
        }

        _handle.anchoredPosition = _input * radius * _handleRange;

        Direction = _input;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _input = Vector2.zero;
        Direction = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
    }
}
