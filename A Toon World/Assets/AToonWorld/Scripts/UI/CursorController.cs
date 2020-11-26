using UnityEngine;

public class CursorController : MonoBehaviour
{
    public enum CursorType { Game, Menu}

    [SerializeField] private Texture2D _menuCursor = null;
    [SerializeField] private Vector2 _hotspot = Vector2.zero;

    void Awake()
    {
        Cursor.SetCursor(_menuCursor, _hotspot == null ? Vector2.zero : _hotspot, CursorMode.Auto);
        Events.InterfaceEvents.CursorChangeRequest.AddListener(cursorType => ChangeCursor(cursorType));
    }

    void Update()
    {
        this.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void ChangeCursor(CursorType cursorType)
    {
        bool gameCursor = cursorType == CursorType.Game;
        this.gameObject.SetActive(gameCursor);
        Cursor.visible = !gameCursor;
    }
}
