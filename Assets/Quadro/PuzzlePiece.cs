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
        ClampToCanvas();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // se estiver perto do slot correto, encaixa
        if (Vector2.Distance(_rect.anchoredPosition, _correctSlot) < 30f)
        {
            _rect.anchoredPosition = _correctSlot;

            // coloca a peça dentro do container "Slots"
            if (_mgr != null && _mgr._slotsContainer != null)
            {
                _rect.SetParent(_mgr._slotsContainer, false);
            }
            else
            {
                // fallback: só joga pro topo da hierarquia do Canvas
                _rect.SetAsLastSibling();
            }

            // desliga o script pra não mover mais
            this.enabled = false;

            // avisa o manager que encaixou
            _mgr.PiecePlaced();
        }
    }

    private void ClampToCanvas()
    {
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        Vector2 pos = _rect.anchoredPosition;

        Vector2 halfCanvas = canvasRect.rect.size * 0.5f;
        Vector2 halfPiece = _rect.rect.size * 0.5f;

        pos.x = Mathf.Clamp(pos.x, -halfCanvas.x + halfPiece.x, halfCanvas.x - halfPiece.x);
        pos.y = Mathf.Clamp(pos.y, -halfCanvas.y + halfPiece.y, halfCanvas.y - halfPiece.y);

        _rect.anchoredPosition = pos;
    }
}
