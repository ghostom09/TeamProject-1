using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class InGameUIManager : MonoBehaviour
{
    public event Action on_esc;
    public InputActionReference cancelAction;
    
    [SerializeField] private Image healthBar;
    [SerializeField] private Image backBar;
    [SerializeField] private Image experienceBar;
    [SerializeField] private Image ultraBar;
    [SerializeField] private List<Image> skillIcons;
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI levelText;
    
    private float lerpSpeed = 20f;
    
    private float displayedHealth;
    private float displayedExperience;
    private float displayedUltra;
    
    private Coroutine delayRoutine;
    private Coroutine AutoFill;
    

    private void Awake()
    {
        displayedHealth = 0;
        displayedUltra = 0;
        displayedExperience = 0;
    }
    void OnEnable()
    {
        cancelAction.action.Enable();
        cancelAction.action.performed += OnStop;
    }

    void OnDisable()
    {
        cancelAction.action.performed -= OnStop;
        cancelAction.action.Disable();
    }

    void OnStop(InputAction.CallbackContext context)
    {
        on_esc?.Invoke();
    }

    public void UpdateLevel(int level)
    {
        levelText.SetText($"Level : {level}");
    }

    public void UpdateProfile(Sprite sprite)
    {
        profile.sprite = sprite;
    }

    public void UpdateSkillIcon(List<Sprite> sprites)
    {
        int count = Mathf.Min(skillIcons.Count, sprites.Count);

        for (int i = 0; i < count; i++)
        {
            skillIcons[i].sprite = sprites[i];
        }

    }
    public void UpdateHealth(int max, int now)
    {
        float targetHP = Mathf.Clamp(now, 0, max);

        healthBar.fillAmount = (float)targetHP / max;

        if (displayedHealth <= 0) displayedHealth = max;

        if (delayRoutine != null)
        {
            StopCoroutine(delayRoutine);
        }

        delayRoutine = StartCoroutine(DelayedLerp(max, targetHP));
    }

    private IEnumerator DelayedLerp(int max, float targetHP)
    {
        yield return new WaitForSeconds(0.3f);

        while (Mathf.Abs(displayedHealth - targetHP) > 0.01f)
        {
            float diffRatio = Mathf.Abs(displayedHealth - targetHP) / max;

            float dynamicSpeed = lerpSpeed * Mathf.Lerp(1f, 3f, diffRatio);  

            displayedHealth = Mathf.MoveTowards(
                displayedHealth, 
                targetHP, 
                dynamicSpeed * Time.deltaTime
            );

            backBar.fillAmount = displayedHealth / max;
            yield return null;
        }

        delayRoutine = null;
    }

    public void UpdateExperience(int max, int currentExp)
    {
        displayedExperience = Mathf.Clamp(currentExp, 0, max);
        experienceBar.fillAmount = displayedExperience / (float)max;
    }
    
    public void UpdateUltimate(int max, int added)
    {
        displayedUltra += added;
        displayedUltra = Mathf.Clamp(displayedUltra, 0, max);
    
        ultraBar.fillAmount = displayedUltra / (float)max;

        if (displayedUltra < max && AutoFill == null)
        {
            AutoFill = StartCoroutine(AutoFillRoutine(max));
        }
    }

    private IEnumerator AutoFillRoutine(int max)
    {
        while (displayedUltra < max)
        {
            displayedUltra += 1;
            displayedUltra = Mathf.Min(displayedUltra, max);
            ultraBar.fillAmount = displayedUltra / max;
            yield return new WaitForSeconds(1f);
        }
        AutoFill = null;
    }
}