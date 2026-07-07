using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SkillTimer : MonoBehaviour
{
    [SerializeField] private Image skillTimerEffect;
    [SerializeField] private TextMeshProUGUI skillTimerText;
    [SerializeField] private Image skillIcon;
    private float skillTime;
    private float cooldownEndTime;
    private bool isCoolingDown;
    private bool isLocked;
    private Coroutine timer;

    private void Awake()
    {
        EnsureSkillIcon();
    }

    public void SetIcon(Sprite sprite)
    {
        EnsureSkillIcon();

        if (skillIcon == null)
            return;

        skillIcon.sprite = sprite;
        skillIcon.enabled = sprite != null;
        skillIcon.preserveAspect = true;
        UpdateLockView();
        skillIcon.transform.SetAsFirstSibling();
    }

    public void SetLocked(bool locked)
    {
        isLocked = locked;
        UpdateLockView();
    }

    private void UpdateLockView()
    {
        if (skillIcon == null)
            return;

        skillIcon.color = isLocked ? Color.black : Color.white;
    }

    private void EnsureSkillIcon()
    {
        if (skillIcon != null && skillIcon.gameObject != gameObject)
            return;

        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            if (image.gameObject == gameObject || image == skillTimerEffect)
                continue;

            if (image.sprite == null)
            {
                skillIcon = image;
                return;
            }
        }
    }

    public void Timer(float skill)
    {
        float newSkillTime = Mathf.Max(0.1f, skill);

        if (isCoolingDown)
        {
            float remain = Mathf.Max(0f, cooldownEndTime - Time.time);
            float elapsed = Mathf.Max(0f, skillTime - remain);

            skillTime = newSkillTime;
            cooldownEndTime = Time.time + Mathf.Max(0f, skillTime - elapsed);
            return;
        }

        skillTime = newSkillTime;
        ResetTimerView();
    }

    public bool UseSkill()
    {
        if (isCoolingDown) return false;
        if (timer != null)
            StopCoroutine(timer);
        timer = StartCoroutine(SkillTimerRoutine());
        return true;
    }

    private IEnumerator SkillTimerRoutine()
    {
        isCoolingDown = true;
        cooldownEndTime = Time.time + skillTime;

        while (Time.time < cooldownEndTime)
        {
            float remain = Mathf.Max(0f, cooldownEndTime - Time.time);

            skillTimerText.SetText(Mathf.CeilToInt(remain).ToString());
            skillTimerEffect.fillAmount = skillTime > 0f ? remain / skillTime : 0f;

            yield return null;
        }

        isCoolingDown = false;
        ResetTimerView();

        timer = null;
    }

    private void ResetTimerView()
    {
        skillTimerText.SetText("");
        skillTimerEffect.fillAmount = 0f;
    }
}
