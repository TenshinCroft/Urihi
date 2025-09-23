using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class EnndingCutscene : MonoBehaviour
{
    [Header("Fade")]
    public Image telaPreta; // arraste a Image preta do Canvas
    public float fadeDuration = 1.5f;

    [Header("Textos")]
    public TextMeshProUGUI[] textos; // arraste os 3 textos
    public float delayEntreTextos = 2f;

    private void Start()
    {
        // Garante que a tela comece invisível
        if (telaPreta != null)
        {
            Color c = telaPreta.color;
            c.a = 0f;
            telaPreta.color = c;
        }

        foreach (var t in textos)
        {
            if (t != null)
            {
                Color c = t.color;
                c.a = 0f;
                t.color = c;
            }
        }
    }

    public void IniciarCutscene()
    {
        StartCoroutine(CutsceneRoutine());
    }

    private IEnumerator CutsceneRoutine()
    {
        // 1. FadeOut tela preta (ficar full)
        yield return StartCoroutine(FadeImage(1f));

        // 2. Mostrar textos em sequência
        foreach (var t in textos)
        {
            yield return StartCoroutine(FadeText(t, 1f));
            yield return new WaitForSeconds(delayEntreTextos);
        }

        // 3. Troca de cena final
        SceneManager.LoadScene("tela inicio");
    }

    private IEnumerator FadeImage(float alvo)
    {
        float start = telaPreta.color.a;
        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, alvo, tempo / fadeDuration);
            Color c = telaPreta.color;
            c.a = a;
            telaPreta.color = c;
            yield return null;
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI texto, float alvo)
    {
        float start = texto.color.a;
        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(start, alvo, tempo / fadeDuration);
            Color c = texto.color;
            c.a = a;
            texto.color = c;
            yield return null;
        }
    }
}
