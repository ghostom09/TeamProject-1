using UnityEngine;

[DefaultExecutionOrder(1000)]
public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;

    private float duration;
    private float fadeDuration;
    private float intensity;
    private Vector3 lastOffset;
    private Vector3 lastShakenPosition;

    public static void Shake(float power, float time)
    {
        CameraShake shake = GetOrCreate();
        if (shake == null)
            return;

        shake.intensity = Mathf.Max(shake.intensity, power);
        shake.duration = Mathf.Max(shake.duration, time);
        shake.fadeDuration = Mathf.Max(shake.fadeDuration, time);
    }

    private static CameraShake GetOrCreate()
    {
        if (instance != null)
            return instance;

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            return null;

        instance = mainCamera.GetComponent<CameraShake>();
        if (instance == null)
            instance = mainCamera.gameObject.AddComponent<CameraShake>();

        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        Vector3 basePosition = transform.position;
        if (lastOffset != Vector3.zero)
        {
            bool positionStillHasShake = (transform.position - lastShakenPosition).sqrMagnitude < 0.0001f;
            if (positionStillHasShake)
                basePosition -= lastOffset;

            lastOffset = Vector3.zero;
        }

        if (duration <= 0f)
        {
            transform.position = basePosition;
            return;
        }

        duration -= Time.unscaledDeltaTime;
        float fade = Mathf.Clamp01(duration / Mathf.Max(0.001f, fadeDuration));
        Vector2 offset = Random.insideUnitCircle * (intensity * fade);
        lastOffset = new Vector3(offset.x, offset.y, 0f);
        lastShakenPosition = basePosition + lastOffset;
        transform.position = lastShakenPosition;

        if (duration <= 0f)
        {
            intensity = 0f;
            fadeDuration = 0f;
        }
    }
}
