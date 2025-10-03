using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public enum LightMode { None, Color, Off }
public enum LightEndMode { None, Color, Off }
public enum TimerMode { Primary, Secondary }
public enum FlashlightEndMode { Normal, Off, On }
public enum EnemyEndMode { Off, On }

public enum PostProcessMode { None, Change, Off }
public enum PostProcessEndMode { None, Change, Off }

[System.Serializable]
public class LightConfig
{
    public Light[] lights;
    public LightMode lightMode = LightMode.None;
    public LightEndMode endMode = LightEndMode.None;
public bool changeColor = false;
    public Color newColor = Color.red;

    public bool changeIntensity = false;
    public float newIntensity = 1f;

    public bool changeRange = false;
    public float newRange = 10f;

    public TimerMode timerMode = TimerMode.Primary;

    [HideInInspector] public Color[] origColors;
    [HideInInspector] public float[] origIntensities;
    [HideInInspector] public float[] origRanges;

}

[System.Serializable]
public class EnemyConfig
{
    public bool enabled = true;
    public Transform spawnPoint;
    public float appearDelay = 0.5f;
    public EnemyEndMode endMode = EnemyEndMode.On;
    public TimerMode timerMode = TimerMode.Primary;
}

[System.Serializable]
public class FlashlightConfig
{
    public enum FlashlightMode { Normal, Primary, Secondary }
    public FlashlightMode mode = FlashlightMode.Normal;
    public FlashlightEndMode endMode = FlashlightEndMode.Normal;
}

[System.Serializable]
public class PostProcessConfig
{
    public PostProcessMode mode = PostProcessMode.None;
    public PostProcessEndMode endMode = PostProcessEndMode.None;
    public float delay = 0f;
    public TimerMode timerMode = TimerMode.Primary;
}

[System.Serializable]
public class ShakeConfig
{
    public bool enabled = false;
    public float strength = 0.3f;
    public TimerMode timerMode = TimerMode.Primary;
}

public class EnemyTriggerEvent : MonoBehaviour
{
    [Header("Geral")]
    public GameObject enemy;
    public GameObject mainCamera;
[Header("Duração do Evento")]
    public float primaryDuration = 5f;
    public float secondaryDuration = 3f;
    public bool repeatableEvent = false;
    private bool _alreadyPlayed = false;

    [Header("Configs Gerais")]
    public LightConfig lightConfig;
    public EnemyConfig enemyConfig;
    public FlashlightConfig flashlightConfig;
    public PostProcessConfig postConfig;
    public ShakeConfig shakeConfig;

    private Renderer[] enemyRenderers;
    private NavMeshAgent enemyNav;
    private CharacterController enemyCC;
    private MonoBehaviour[] enemyScripts;

    private lanterna flashlightScript;
    private Player playerScript;
    private ScreenShake screenShake;
    private PostProcessController postController;

    private void Awake()
    {
        if (enemy != null)
        {
            enemyRenderers = enemy.GetComponentsInChildren<Renderer>(true);
            enemyNav = enemy.GetComponent<NavMeshAgent>();
            enemyCC = enemy.GetComponent<CharacterController>();
            enemyScripts = enemy.GetComponents<MonoBehaviour>();
            SetEnemyVisible(false);
        }

        if (mainCamera != null)
        {
            flashlightScript = mainCamera.GetComponent<lanterna>();
            screenShake = mainCamera.GetComponent<ScreenShake>();
            postController = mainCamera.GetComponentInChildren<PostProcessController>();

            // fallback se não achar no mainCamera
            if (postController == null)
                postController = FindObjectOfType<PostProcessController>();

            if (flashlightScript != null && flashlightScript._pObj != null)
                playerScript = flashlightScript._pObj.GetComponent<Player>();
        }

        SaveLightState(lightConfig);
    }

    private void OnValidate()
    {
        if (repeatableEvent)
        {
            _alreadyPlayed = false;
            if (TryGetComponent<Collider>(out Collider col))
                col.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_alreadyPlayed || repeatableEvent)
            {
                StartCoroutine(RunEvent());
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    private IEnumerator RunEvent()
    {
        float duration = primaryDuration;

        if (flashlightConfig.mode != FlashlightConfig.FlashlightMode.Normal && flashlightScript != null)
            StartCoroutine(HandleFlashlight(duration));

        if (postConfig.mode != PostProcessMode.None)
            StartCoroutine(HandlePost(duration));

        if (shakeConfig.enabled && screenShake != null)
            StartCoroutine(HandleShake(duration));

        if (lightConfig.lights.Length > 0)
            ApplyLightChanges(lightConfig);

        yield return new WaitForSeconds(enemyConfig.appearDelay);

        if (enemyConfig.enabled && enemy != null)
        {
            if (enemyConfig.spawnPoint != null)
            {
                enemy.transform.position = enemyConfig.spawnPoint.position;
                enemy.transform.rotation = enemyConfig.spawnPoint.rotation;
            }
            SetEnemyVisible(true);
        }

        yield return new WaitForSeconds(duration);

        HandleEndModes(lightConfig, flashlightConfig, enemyConfig, postConfig);

        _alreadyPlayed = true;

        if (repeatableEvent)
        {
            yield return new WaitForSeconds(0.1f);
            _alreadyPlayed = false;
            GetComponent<Collider>().enabled = true;
        }
    }

    private void HandleEndModes(LightConfig lightCfg, FlashlightConfig flashCfg, EnemyConfig enemyCfg, PostProcessConfig postCfg)
    {
        // luzes
        if (lightCfg.endMode == LightEndMode.None) RestoreLights(lightCfg);
        else if (lightCfg.endMode == LightEndMode.Off) TurnLightsOff(lightCfg);

        // lanterna do player
        if (flashCfg.endMode == FlashlightEndMode.Off && playerScript != null)
            playerScript._lntrOn = false;
        else if (flashCfg.endMode == FlashlightEndMode.On && playerScript != null)
            playerScript._lntrOn = true;

        // inimigo
        if (enemyCfg.endMode == EnemyEndMode.Off) SetEnemyVisible(false);
        else if (enemyCfg.endMode == EnemyEndMode.On) SetEnemyVisible(true);

        // post processing — sempre tratar se o evento mexeu com ele
        if (postCfg.mode != PostProcessMode.None)
            HandlePostEndMode();
    }

    private void HandlePostEndMode()
    {
        if (postController == null) return;

        if (postConfig.endMode == PostProcessEndMode.None)
            postController.SetOriginal();
        else if (postConfig.endMode == PostProcessEndMode.Change)
            postController.SetAlt(true);
        else if (postConfig.endMode == PostProcessEndMode.Off)
            postController.SetOff();
    }

    private void SaveLightState(LightConfig lightCfg)
    {
        if (lightCfg.lights == null || lightCfg.lights.Length == 0) return;
        int count = lightCfg.lights.Length;
        lightCfg.origColors = new Color[count];
        lightCfg.origIntensities = new float[count];
        lightCfg.origRanges = new float[count];

        for (int i = 0; i < count; i++)
        {
            Light l = lightCfg.lights[i];
            if (l != null)
            {
                lightCfg.origColors[i] = l.color;
                lightCfg.origIntensities[i] = l.intensity;
                lightCfg.origRanges[i] = l.range;
            }
        }
    }

    private void ApplyLightChanges(LightConfig lightCfg)
    {
        if (lightCfg.lightMode == LightMode.None) return;

        foreach (Light l in lightCfg.lights)
        {
            if (l == null) continue;
            if (lightCfg.lightMode == LightMode.Color)
            {
                if (lightCfg.changeColor) l.color = lightCfg.newColor;
                if (lightCfg.changeIntensity) l.intensity = lightCfg.newIntensity;
                if (lightCfg.changeRange) l.range = lightCfg.newRange;
                l.enabled = true;
            }
            else if (lightCfg.lightMode == LightMode.Off)
            {
                l.enabled = false;
            }
        }
    }

    private void RestoreLights(LightConfig lightCfg)
    {
        if (lightCfg.lights == null) return;
        for (int i = 0; i < lightCfg.lights.Length; i++)
        {
            if (lightCfg.lights[i] == null) continue;
            lightCfg.lights[i].color = lightCfg.origColors[i];
            lightCfg.lights[i].intensity = lightCfg.origIntensities[i];
            lightCfg.lights[i].range = lightCfg.origRanges[i];
            lightCfg.lights[i].enabled = true;
        }
    }

    private void TurnLightsOff(LightConfig lightCfg)
    {
        foreach (Light l in lightCfg.lights)
            if (l != null) l.enabled = false;
    }

    private void SetEnemyVisible(bool visible)
    {
        if (enemyRenderers != null)
            foreach (Renderer r in enemyRenderers) r.enabled = visible;

        if (enemyNav != null) enemyNav.enabled = visible;
        if (enemyCC != null) enemyCC.enabled = visible;

        if (enemyScripts != null)
            foreach (MonoBehaviour script in enemyScripts)
                if (script != this) script.enabled = visible;
    }

    private IEnumerator HandleFlashlight(float duration)
    {
        if (playerScript == null) yield break;

        bool originalState = playerScript._lntrOn;
        playerScript._lntrOn = false;

        float waitTime = (flashlightConfig.mode == FlashlightConfig.FlashlightMode.Secondary) ? secondaryDuration : duration;
        yield return new WaitForSeconds(waitTime);

        if (flashlightConfig.endMode == FlashlightEndMode.Normal)
            playerScript._lntrOn = originalState;
    }

    private IEnumerator HandlePost(float duration)
    {
        if (postController == null) yield break;

        if (postConfig.delay > 0f) yield return new WaitForSeconds(postConfig.delay);

        if (postConfig.mode == PostProcessMode.Change)
            postController.SetAlt(true);
        else if (postConfig.mode == PostProcessMode.Off)
            postController.SetOff();

        float waitTime = (postConfig.timerMode == TimerMode.Secondary) ? secondaryDuration : duration;
        yield return new WaitForSeconds(waitTime);

        HandlePostEndMode();
    }

    private IEnumerator HandleShake(float duration)
    {
        if (screenShake == null) yield break;
        float waitTime = (shakeConfig.timerMode == TimerMode.Secondary) ? secondaryDuration : duration;
        screenShake.Shake(waitTime, shakeConfig.strength);
        yield return new WaitForSeconds(waitTime);
    }

}
