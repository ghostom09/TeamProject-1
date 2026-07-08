using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class GameOverButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.04f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float animationSpeed = 14f;
    [SerializeField] private Color hoverTint = new Color(1f, 0.36f, 0.36f, 1f);

    private RectTransform rectTransform;
    private Graphic targetGraphic;
    private Color normalColor;
    private Coroutine animationRoutine;
    private bool isHovering;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        targetGraphic = GetComponent<Graphic>();
        if (targetGraphic != null)
            normalColor = targetGraphic.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        AnimateTo(hoverScale, hoverTint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        AnimateTo(1f, normalColor);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        AnimateTo(pressedScale, normalColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        AnimateTo(isHovering ? hoverScale : 1f, isHovering ? hoverTint : normalColor);
    }

    private void AnimateTo(float scale, Color color)
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(AnimateRoutine(scale, color));
    }

    private IEnumerator AnimateRoutine(float scale, Color color)
    {
        Vector3 targetScale = Vector3.one * scale;

        while (Vector3.Distance(rectTransform.localScale, targetScale) > 0.001f)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
            if (targetGraphic != null)
                targetGraphic.color = Color.Lerp(targetGraphic.color, color, Time.unscaledDeltaTime * animationSpeed);

            yield return null;
        }

        rectTransform.localScale = targetScale;
        if (targetGraphic != null)
            targetGraphic.color = color;

        animationRoutine = null;
    }
}
