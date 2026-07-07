using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum SceneName
{
    MainMenu,
    PlayerChoice,
    InGame
}
public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    [SerializeField] private float sceneFadeOutDuration = 0.25f;
    [SerializeField] private float sceneFadeInDuration = 0.15f;

    private Image fadeImage;
    private CanvasGroup fadeGroup;
    private Coroutine sceneChangeRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureFadeOverlay();
        }
        else Destroy(gameObject);
    }

    public void ChangeScene(SceneName sceneName)
    {
        ChangeScene(sceneName.ToString());
    }

    public void ChangeScene(string sceneName)
    {
        if (sceneChangeRoutine != null)
            return;

        sceneChangeRoutine = StartCoroutine(ChangeSceneRoutine(sceneName));
    }

    private IEnumerator ChangeSceneRoutine(string sceneName)
    {
        EnsureFadeOverlay();
        fadeGroup.blocksRaycasts = true;

        yield return Fade(0f, 1f, sceneFadeOutDuration);

        Time.timeScale = 1f;
        AudioListener.pause = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

        yield return null;
        yield return Fade(1f, 0f, sceneFadeInDuration);

        fadeGroup.blocksRaycasts = false;
        sceneChangeRoutine = null;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float timer = 0f;
        fadeGroup.alpha = from;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = duration <= 0f ? 1f : Mathf.Clamp01(timer / duration);
            fadeGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        fadeGroup.alpha = to;
    }

    private void EnsureFadeOverlay()
    {
        if (fadeGroup != null)
            return;

        GameObject canvasObj = new GameObject("SceneFadeCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasGroup));
        canvasObj.transform.SetParent(transform, false);

        Canvas fadeCanvas = canvasObj.GetComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.overrideSorting = true;
        fadeCanvas.sortingOrder = 32766;

        fadeGroup = canvasObj.GetComponent<CanvasGroup>();
        fadeGroup.alpha = 0f;
        fadeGroup.blocksRaycasts = false;
        fadeGroup.interactable = false;

        GameObject imageObj = new GameObject("SceneFadeImage", typeof(RectTransform), typeof(Image));
        imageObj.transform.SetParent(canvasObj.transform, false);

        RectTransform imageRt = imageObj.GetComponent<RectTransform>();
        imageRt.anchorMin = Vector2.zero;
        imageRt.anchorMax = Vector2.one;
        imageRt.offsetMin = Vector2.zero;
        imageRt.offsetMax = Vector2.zero;

        fadeImage = imageObj.GetComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = true;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
