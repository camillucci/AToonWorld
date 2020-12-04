using UnityEngine;
using static PlayerInkController;

public class CursorController : MonoBehaviour
{
    public enum CursorType { Game, Menu, None}

    [SerializeField] private Texture2D _menuCursor = null;
    [SerializeField] private Vector2 _hotspot = Vector2.zero;
    [SerializeField] private InkPaletteSO _inkPalette = null;

    private TrailRenderer _trailRenderer;

    void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
        Cursor.SetCursor(_menuCursor, _hotspot == null ? Vector2.zero : _hotspot, CursorMode.Auto);
        Events.InterfaceEvents.CursorChangeRequest.AddListener(ChangeCursor);
        Events.InterfaceEvents.InkSelected.AddListener(OnInkChanged);
    }

    private void OnDestroy() 
    {
        Events.InterfaceEvents.CursorChangeRequest.RemoveListener(ChangeCursor);
        Events.InterfaceEvents.InkSelected.RemoveListener(OnInkChanged);
    }

    void Update()
    {
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ChangeCursor(CursorType cursorType)
    {
        bool gameCursor = cursorType == CursorType.Game;
        _trailRenderer.Clear();
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.gameObject.SetActive(gameCursor);
        Cursor.visible = cursorType == CursorType.Menu;
    }

    private void OnInkChanged(InkType inkType)
    {
        if(_inkPalette != null)
        {
            foreach(ScriptableInkHandler ink in _inkPalette.InkPalette)
            {
                if(ink.InkType == inkType)
                {
                    _trailRenderer.startColor = ink.InkColor;
                    _trailRenderer.endColor = ink.InkColor;
                    return;
                }
            }
        }
    }
}
