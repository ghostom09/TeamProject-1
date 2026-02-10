using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ArgumentDisplay : MonoBehaviour
{
    public ItemData items;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image icon;
    [SerializeField]private Image canvas;

    public void SetArgument(ItemData data)
    {
        if (data == null) 
            return;
        
        items = data;
        name.SetText(items.itemName);
        description.SetText(items.description);
        icon.sprite = items.icon;
    }

    public void ApplyArgument()
    {
        ArgumentManager.Instance.Calculate(items);;
    }
    
    
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(duration));
    }

    private IEnumerator FadeCoroutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(1f, 0f, t);

            canvas.color      = new Color(1f, 1f, 1f, alpha);
            icon.color        = new Color(1f, 1f, 1f, alpha);
            name.color        = new Color(1f, 1f, 1f, alpha);
            description.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        canvas.color      = new Color(1f, 1f, 1f, 0f);
        icon.color        = new Color(1f, 1f, 1f, 0f);
        name.color        = new Color(1f, 1f, 1f, 0f);
        description.color = new Color(1f, 1f, 1f, 0f);

        Destroy(gameObject);
    }
    

}
