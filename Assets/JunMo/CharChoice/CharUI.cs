using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class CharUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button[] characterButtons;
    [SerializeField] private RectTransform charactersPanel;
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private CharacterData[] characters;
    [SerializeField] private GameObject errorPanel;
    
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
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i;
            characterButtons[i].onClick.AddListener(() => OnClickButton(index));
        }
    }
    void OnClickButton(int index)
    {
        characterData = characters[index];
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
        if (!characterData)
        {
            StartCoroutine(Error());
            return;
        }
        CharDataManager.Instance.GetData(characterData);
        SceneManager.Instance.ChangeScene(SceneName.InGame);
    }

    private IEnumerator Error()
    {
        errorPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        errorPanel.SetActive(false);
    }
}
