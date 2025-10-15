using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;

    private void Awake() => _originalPos = transform.localPosition;

    public void Shake(float duration, float magnitude)
    {
        // só chacoalha se efeitos estiverem ativos e jogo não estiver pausado
        if (SettingsMenu.isPaused || !SettingsMenu.effectsEnabled)
        {
            ResetCameraPosition();
            return;
        }

        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (SettingsMenu.isPaused || !SettingsMenu.effectsEnabled)
            {
                yield return null;
                continue;
            }

            // CORREÇÃO: Removemos o cálculo de fade out para garantir a força máxima do shake.
            // O screenshake agora usa a 'magnitude' total durante toda a 'duration'.

            // Aplica a posição original mais o offset do shake, usando a magnitude total
            transform.localPosition = _originalPos + new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0
            );

            // Usa Time.unscaledDeltaTime para que o shake termine no tempo real.
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Chamada de reset explícita ao final da corrotina para garantir a estabilidade.
        ResetCameraPosition();
    }

    public void ResetCameraPosition()
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        // Força o localPosition de volta ao original.
        if (transform.localPosition != _originalPos)
        {
            transform.localPosition = _originalPos;
            Debug.Log("Posição da câmera resetada para estabilidade.");
        }
    }
}