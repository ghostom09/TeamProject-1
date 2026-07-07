using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SkillTimer : MonoBehaviour
{
    [SerializeField] private Image skillTimerEffect;
    [SerializeField] private TextMeshProUGUI skillTimerText;
    [SerializeField] private Image skillIcon;
    private int skillTime;
    private int nowSkillTime = 0;
    private Coroutine timer;

    public void Timer(int skill)
    {
        skillTime = skill;
        nowSkillTime = 0;
        skillTimerText.SetText("");
        skillTimerEffect.fillAmount = 0f;
    }

    public bool UseSkill()
    {
        if (nowSkillTime > 0) return false;
        if (timer != null)
            StopCoroutine(timer);
        timer = StartCoroutine(SkillTimerRoutine());
        return true;
    }

    private IEnumerator SkillTimerRoutine()
    {
        nowSkillTime = skillTime;

        while (nowSkillTime > 0)
        {
            skillTimerText.SetText(nowSkillTime.ToString());

            skillTimerEffect.fillAmount = (float)nowSkillTime / skillTime;

            yield return new WaitForSeconds(1f);
            nowSkillTime--;
        }
        skillTimerText.SetText("");
        skillTimerEffect.fillAmount = 0;

        timer = null;
    }
}
