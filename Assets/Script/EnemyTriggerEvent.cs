using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyTriggerEvent : MonoBehaviour
{
    [Header("Configuração do Evento")]
    public EventType eventToRun = EventType.Event1;
    public enum EventType { Event1, Event2 }

    [Header("Referências Gerais")]
    public GameObject enemy;
    public GameObject playerFlashlight;

    [Header("Configuração da Lanterna")]
    public bool disableFlashlight = false;
    public float flashlightDisableTime = 3f;
    private bool _flshOriginalState;

    // ---------------------
    // POST PROCESSING
    // ---------------------
    [Header("Post Processing")]
    public bool affectPostProcessing = false;
    public bool disablePostInstead = false;   // se true, só desliga o volume
    public GameObject postProcessingOriginal;
    public GameObject alternatePostProcessing;

    public float delayBeforePost = 0f;
    public float postActiveTime = 2f;
    private bool postIsActive = false;

    // ---------------------
    // SCREEN SHAKE
    // ---------------------
    [Header("Screen Shake")]
    public ScreenShake screenShake;
    public bool useScreenShake = false;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.3f;

    // ---------------------
    // EVENTO 1 CONFIG
    // ---------------------
    [Header("Evento 1 - Luzes e Inimigo")]
    public Light[] lightsToToggle;
    public float delayBeforeEnemy = 0.5f;
    public float enemyVisibleTime = 2f;
    public float delayAfterEnemy = 2f;
    public float delayBeforeLight = 0f;
    public bool useColorChange = false;
    public Color targetColor = Color.red;
    public Color originalLightColor = Color.white;
    private Color[] savedOriginalColors;

    // ---------------------
    // EVENTO 2 CONFIG
    // ---------------------
    [Header("Evento 2 - Luzes e Spawn Alternativo")]
    public Light[] lightsForEvent2;
    public bool event2UseColorChange = false;
    public Color event2TargetColor = Color.blue;
    public Color event2OriginalLightColor = Color.white;
    public Transform enemySpawnPoint;
    public float event2VisibleTime = 5f;

    private Color[] savedEvent2Colors;

    // refs do inimigo
    private Renderer[] enemyRenderers;
    private NavMeshAgent enemyNav;
    private MonoBehaviour[] enemyScripts;
    private Collider[] enemyColliders;

    private void Awake()
    {
        if (enemy != null)
        {
            enemyRenderers = enemy.GetComponentsInChildren<Renderer>(true);
            enemyNav = enemy.GetComponent<NavMeshAgent>();
            enemyScripts = enemy.GetComponents<MonoBehaviour>();
            enemyColliders = enemy.GetComponentsInChildren<Collider>(true); // pega todos os colliders
            SetEnemyVisible(false);
        }

        if (lightsToToggle != null && lightsToToggle.Length > 0)
        {
            savedOriginalColors = new Color[lightsToToggle.Length];
            for (int i = 0; i < lightsToToggle.Length; i++)
                if (lightsToToggle[i] != null)
                    savedOriginalColors[i] = lightsToToggle[i].color;
        }

        if (lightsForEvent2 != null && lightsForEvent2.Length > 0)
        {
            savedEvent2Colors = new Color[lightsForEvent2.Length];
            for (int i = 0; i < lightsForEvent2.Length; i++)
                if (lightsForEvent2[i] != null)
                    savedEvent2Colors[i] = lightsForEvent2[i].color;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (eventToRun)
            {
                case EventType.Event1:
                    StartCoroutine(Event1());
                    break;
                case EventType.Event2:
                    StartCoroutine(Event2());
                    break;
            }
            GetComponent<Collider>().enabled = false;
        }
    }

    // -------------------
    // EVENTO 1
    // -------------------
    private IEnumerator Event1()
    {
        if (disableFlashlight && playerFlashlight != null)
            StartCoroutine(DisableFlashlightTemporarily());

        if (affectPostProcessing)
            StartCoroutine(HandlePostProcessing());

        if (useScreenShake && screenShake != null)
            screenShake.Shake(shakeDuration, shakeMagnitude);

        if (delayBeforeLight > 0f)
            yield return new WaitForSeconds(delayBeforeLight);

        if (useColorChange)
        {
            for (int i = 0; i < lightsToToggle.Length; i++)
                if (lightsToToggle[i] != null)
                    lightsToToggle[i].color = targetColor;
        }
        else
        {
            foreach (Light l in lightsToToggle)
                if (l != null) l.enabled = false;
        }

        yield return new WaitForSeconds(delayBeforeEnemy);

        SetEnemyVisible(true);

        yield return new WaitForSeconds(enemyVisibleTime);

        SetEnemyVisible(false);

        yield return new WaitForSeconds(delayAfterEnemy);

        if (useColorChange)
        {
            for (int i = 0; i < lightsToToggle.Length; i++)
                if (lightsToToggle[i] != null)
                    lightsToToggle[i].color = savedOriginalColors[i];
        }
        else
        {
            foreach (Light l in lightsToToggle)
                if (l != null) l.enabled = true;
        }
        if (enemy != null)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null) e.footstepsEnabled = true;
        }
        if (enemy != null)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null) e.footstepsEnabled = false;
        }
    }

    // -------------------
    // EVENTO 2
    // -------------------
    private IEnumerator Event2()
    {
        if (disableFlashlight && playerFlashlight != null)
            StartCoroutine(DisableFlashlightTemporarily());

        if (affectPostProcessing)
            StartCoroutine(HandlePostProcessing());

        if (useScreenShake && screenShake != null)
            screenShake.Shake(shakeDuration, shakeMagnitude);

        if (event2UseColorChange)
        {
            for (int i = 0; i < lightsForEvent2.Length; i++)
                if (lightsForEvent2[i] != null)
                    lightsForEvent2[i].color = event2TargetColor;
        }
        else
        {
            foreach (Light l in lightsForEvent2)
                if (l != null) l.enabled = false;
        }
        if (enemy != null)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null) e.footstepsEnabled = true;
        }
        ChaseMusicController.instance?.EnableMusicAfterEvent2();

        if (enemySpawnPoint != null && enemy != null)
        {
            enemy.transform.position = enemySpawnPoint.position;
            enemy.transform.rotation = enemySpawnPoint.rotation;
        }

        SetEnemyVisible(true);

        yield return new WaitForSeconds(event2VisibleTime);

        if (event2UseColorChange)
        {
            for (int i = 0; i < lightsForEvent2.Length; i++)
                if (lightsForEvent2[i] != null)
                    lightsForEvent2[i].color = savedEvent2Colors[i];
        }
        else
        {
            foreach (Light l in lightsForEvent2)
                if (l != null) l.enabled = true;
        }
    }

    // -------------------
    // AUXILIARES
    // -------------------
    private void SetEnemyVisible(bool visible)
    {
        if (enemyRenderers != null)
            foreach (Renderer r in enemyRenderers) r.enabled = visible;

        if (enemyNav != null) enemyNav.enabled = visible;

        if (enemyScripts != null)
            foreach (MonoBehaviour script in enemyScripts)
                if (script != this) script.enabled = visible;

        if (enemyColliders != null) // aqui desligamos os colliders
            foreach (Collider c in enemyColliders)
                c.enabled = visible;
    }

    private IEnumerator DisableFlashlightTemporarily()
    {
        _flshOriginalState = playerFlashlight.activeSelf;
        playerFlashlight.SetActive(false);
        yield return new WaitForSeconds(flashlightDisableTime);
        playerFlashlight.SetActive(_flshOriginalState);
    }

    private IEnumerator HandlePostProcessing()
    {
        if (delayBeforePost > 0f)
            yield return new WaitForSeconds(delayBeforePost);

        TogglePostProcessing(true);
        yield return new WaitForSeconds(postActiveTime);
        TogglePostProcessing(false);
    }

    private void TogglePostProcessing(bool activateAlt)
    {
        if (disablePostInstead)
        {
            if (postProcessingOriginal != null)
                postProcessingOriginal.SetActive(!activateAlt);
        }
        else
        {
            if (postProcessingOriginal != null)
                postProcessingOriginal.SetActive(!activateAlt);
            if (alternatePostProcessing != null)
                alternatePostProcessing.SetActive(activateAlt);
        }
        postIsActive = activateAlt;
    }
}
