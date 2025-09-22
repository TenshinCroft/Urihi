using UnityEngine;
using System.Collections;
using UnityEngine.AI;
public class EnemyTrigger : MonoBehaviour

{
    [Header("Referências")]
    public GameObject enemy;
    public Light[] lightsEvent1;
    public Light[] lightsEvent2;
    public AudioSource eventAudio1;
    public AudioSource eventAudio2;
    public Transform enemyPostEvent1Position; // local pós-evento 1
    public Transform enemyPostEvent2Position; // local onde o inimigo aparece no evento 2
    public float event1Duration = 3f;
    public float event2Duration = 3f;

    private bool event1Triggered = false;
    private bool event2Triggered = false;
    private Renderer[] enemyRenderers;
    private Color[] originalColorsEvent1;
    private Color[] originalColorsEvent2;
    private NavMeshAgent enemyNav;
    private Enemy enemyScript;

    private void Awake()
    {
        if (enemy != null)
        {
            // pega componentes do inimigo
            enemyNav = enemy.GetComponent<NavMeshAgent>();
            enemyScript = enemy.GetComponent<Enemy>();

            // inimigo invisível e congelado no início
            if (enemyNav != null) enemyNav.enabled = false;
            if (enemyScript != null) enemyScript.enabled = false;

            enemyRenderers = enemy.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in enemyRenderers)
                r.enabled = false;
        }

        // cores originais das luzes
        if (lightsEvent1 != null)
        {
            originalColorsEvent1 = new Color[lightsEvent1.Length];
            for (int i = 0; i < lightsEvent1.Length; i++)
                originalColorsEvent1[i] = lightsEvent1[i].color;
        }

        if (lightsEvent2 != null)
        {
            originalColorsEvent2 = new Color[lightsEvent2.Length];
            for (int i = 0; i < lightsEvent2.Length; i++)
                originalColorsEvent2[i] = lightsEvent2[i].color;
        }
    }

    // --- Evento 1 ---
    public void TriggerEvent1()
    {
        if (!event1Triggered)
        {
            StartCoroutine(Event1Routine());
            event1Triggered = true;
        }
    }

    private IEnumerator Event1Routine()
    {
        // inimigo visível
        foreach (Renderer r in enemyRenderers)
            r.enabled = true;

        if (eventAudio1 != null) eventAudio1.Play();

        Coroutine lightsBlink = StartCoroutine(BlinkLights(lightsEvent1));

        yield return new WaitForSeconds(event1Duration);

        StopCoroutine(lightsBlink);

        // luzes voltam ao normal
        for (int i = 0; i < lightsEvent1.Length; i++)
        {
            lightsEvent1[i].color = originalColorsEvent1[i];
            lightsEvent1[i].enabled = true;
        }

        // move inimigo pós-evento 1
        if (enemyPostEvent1Position != null)
        {
            enemy.transform.position = enemyPostEvent1Position.position;
            enemy.transform.rotation = enemyPostEvent1Position.rotation;
        }

        // inimigo invisível e congelado
        foreach (Renderer r in enemyRenderers)
            r.enabled = false;
        if (enemyNav != null) enemyNav.enabled = false;
        if (enemyScript != null) enemyScript.enabled = false;
    }

    // --- Evento 2 ---
    public void TriggerEvent2()
    {
        if (!event2Triggered)
        {
            StartCoroutine(Event2Routine());
            event2Triggered = true;
        }
    }

    private IEnumerator Event2Routine()
    {
        // move inimigo para local do evento 2
        if (enemyPostEvent2Position != null)
        {
            enemy.transform.position = enemyPostEvent2Position.position;
            enemy.transform.rotation = enemyPostEvent2Position.rotation;
        }

        // inimigo visível
        foreach (Renderer r in enemyRenderers)
            r.enabled = true;

        // toca áudio do evento 2
        if (eventAudio2 != null)
            eventAudio2.Play();

        // luzes piscando
        Coroutine lightsBlink2 = StartCoroutine(BlinkLights(lightsEvent2));

        yield return new WaitForSeconds(event2Duration);

        // para piscagem das luzes
        StopCoroutine(lightsBlink2);

        // volta cores originais das luzes
        for (int i = 0; i < lightsEvent2.Length; i++)
        {
            lightsEvent2[i].color = originalColorsEvent2[i];
            lightsEvent2[i].enabled = true;
        }

        // libera inimigo para agir normalmente pelo resto do jogo
        if (enemyNav != null) enemyNav.enabled = true;
        if (enemyScript != null) enemyScript.enabled = true;
    }

    private IEnumerator BlinkLights(Light[] lightsToBlink)
    {
        while (true)
        {
            foreach (Light l in lightsToBlink)
            {
                l.enabled = true;
                l.color = Color.red;
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));

            foreach (Light l in lightsToBlink)
            {
                l.enabled = true;
                l.color = originalColorsEvent2[System.Array.IndexOf(lightsToBlink, l)]; // volta cor original rapidamente
            }
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }
    }
}



