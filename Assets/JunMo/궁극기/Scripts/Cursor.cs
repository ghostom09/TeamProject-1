using UnityEngine;

public class CursorChange : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    public void UseUltra(bool enable)
    {
        Cursor.SetCursor(enable ? cursorTexture : null, hotSpot, cursorMode);
    }
}
