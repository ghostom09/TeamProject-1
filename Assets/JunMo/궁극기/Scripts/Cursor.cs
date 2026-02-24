using UnityEngine;

public class CursorChange : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Vector2 hotSpot = Vector2.zero;   // 클릭 기준점
    public CursorMode cursorMode = CursorMode.Auto;

    public void UseUltra(bool enable) //좀더 이미지 픽셀수 크게
    {
        Cursor.SetCursor(enable ? cursorTexture : null, hotSpot, cursorMode);
    }
}
