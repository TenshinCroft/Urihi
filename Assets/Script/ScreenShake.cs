using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;

    private void Awake() => _originalPos = transform.localPosition;

    public void Shake(float duration, float magnitude)
    {
        if (SettingsMenu.isPaused) return;
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (SettingsMenu.isPaused)
            {
                yield return null;
                continue;
            }

            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originalPos;
    }
}
