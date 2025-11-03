using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    // Chave utilizada para salvar a preferência de Screen Shake no PlayerPrefs
    private const string ScreenShakeKey = "ScreenShakeEnabled";
    private const bool DefaultScreenShake = true; // Valor padrão (ligado)

    private Vector3 _originalPos;
    private Coroutine _shakeCoroutine;

    private void Awake() => _originalPos = transform.localPosition;

    // --- NOVO MÉTODO: Verifica o PlayerPrefs ---
    private bool IsScreenShakeEnabled()
    {
        // PlayerPrefs.GetInt(chave, default) retorna 1 para true, 0 para false.
        // O valor padrão 1 (true) é usado se a chave não existir.
        return PlayerPrefs.GetInt(ScreenShakeKey, DefaultScreenShake ? 1 : 0) == 1;
    }
    // ------------------------------------------

    public void Shake(float duration, float magnitude)
    {
        // 1. Verifica se o Screen Shake está DESATIVADO nas configurações ou o jogo está pausado.
        if (SettingsMenu.isPaused || !IsScreenShakeEnabled())
        {
            ResetCameraPosition();
            Debug.Log("Tentativa de Shake bloqueada: Jogo Pausado ou Screen Shake desativado.");
            return;
        }

        // Se já estiver chacoalhando, para o atual para iniciar o novo (com prioridade)
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 2. Durante o loop, verifica a pausa e a configuração ATUALIZADA
            if (SettingsMenu.isPaused || !IsScreenShakeEnabled())
            {
                // Se for pausado ou desativado no meio do shake, reseta a posição imediatamente e sai do loop.
                ResetCameraPosition();
                yield break;
            }

            // Aplica a posição original mais o offset do shake, usando a magnitude total
            transform.localPosition = _originalPos + new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0
            );

            // Usa Time.unscaledDeltaTime para que o shake termine no tempo real, mesmo se o jogo estiver lento.
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