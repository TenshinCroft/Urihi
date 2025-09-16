using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Vector2 _correctSlot;
    public PuzzleManager _mgr;

    private RectTransform _rect;
    private Canvas _canvas;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    public void OnDrag(PointerEventData eventData)
    {
        _rect.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // se estiver perto do slot correto, encaixa
        if (Vector2.Distance(_rect.anchoredPosition, _correctSlot) < 30f)
        {
            _rect.anchoredPosition = _correctSlot;
            this.enabled = false;
            _mgr.PiecePlaced();
        }
    }
}
