using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CharUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private RectTransform charactersPanel;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private RectTransform viewport;
    
    private CharacterData characterData;

    public int itemCount = 3;

    void Start()
    {
        SetButton();
        Resize();
    }

    private void SetButton()
    {
        startBtn.onClick.AddListener(OnStartGame);
    }

    private void Resize()
    {
        int column = grid.constraintCount;
        if (column == 0) return;
        if(column > 6) column = 6;

        int row = Mathf.CeilToInt((float)itemCount / column);

        float height =
            (row * grid.cellSize.y) +
            ((row - 1) * grid.spacing.y) +
            grid.padding.top +
            grid.padding.bottom;

        viewport.sizeDelta = new Vector2(viewport.sizeDelta.x, height);
    }

    private void OnStartGame()
    {
        CharDataManager.Instance.GetData(characterData);
        SceneManager.Instance.ChangeScene(SceneName.PlayerTest);
    }
}
