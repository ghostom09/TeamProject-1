using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ParadoxController : MonoBehaviour
{
    [SerializeField] private Image fadeOut;
    
    [SerializeField] private EnemyTest enemy;
    [SerializeField] private CursorChange cursor;
    
    [SerializeField]private Volume globalVolume;
    [SerializeField] private Volume localVolume;
    [SerializeField]private ParticleSystem auraParticle;
    private ParticleSystem.EmissionModule emission;
    
    private float fadeOutSpeed = 5f;
    private float fadeInSpeed = 3f;

    void Awake()
    {
        if(auraParticle != null)
            emission = auraParticle.emission;
    }

    public void ResetUltra()
    {
        localVolume.weight = 0;
        globalVolume.weight = 0;
        auraParticle.Stop();
    }

    public void SetVolumeActive(bool active)
    {
        if (active)
        {
            StartCoroutine(GoldenCarnivalRoutine(true));
        }

        else
        {
            StartCoroutine(GoldenCarnivalRoutine(false));
        }
    }

    private IEnumerator GoldenCarnivalRoutine(bool enable)
    {
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha = Mathf.MoveTowards(alpha, 1f, Time.deltaTime * fadeOutSpeed);
            fadeOut.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }
        
        globalVolume.weight = enable ? 1f : 0f;
        localVolume.weight = enable ? 1f : 0f;
        enemy.UseUltra(enable);
        cursor.UseUltra(enable);

        if (enable)
            auraParticle.Play();
        else
            auraParticle.Stop();
        
        while (alpha > 0f)
        {
            alpha = Mathf.MoveTowards(alpha, 0f, Time.deltaTime * fadeInSpeed);
            fadeOut.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}