using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public enum EffectMode { None, Off, KeepEvent }
public enum TimerMode { Global, Custom }
public enum EffectModeLantern { Normal, Off, On }

[System.Serializable]
public class LightConfig
{
    public Light[] lights;
    public bool changeColor = false;
    public Color newColor = Color.red;
    public bool changeIntensity = false;
    public float newIntensity = 1f;
    public bool changeRange = false;
    public float newRange = 10f;
    public TimerMode timerMode = TimerMode.Global;
    public EffectMode endMode = EffectMode.None;

    [HideInInspector] public Color[] origColors;
    [HideInInspector] public float[] origIntensities;
    [HideInInspector] public float[] origRanges;
}

[System.Serializable]
public class EnemyConfig
{
    public bool enableEnemy = true;
    public Transform spawnPoint;
    public float appearDelay = 0.5f;
    public TimerMode timerMode = TimerMode.Global;
    public EffectMode endMode = EffectMode.None;
}

[System.Serializable]
public class FlashlightConfig
{
    public bool disableTemporarily = false;
    public TimerMode timerMode = TimerMode.Global;
    public float disableTime = 3f;
    public EffectModeLantern endMode = EffectModeLantern.Normal;
}

[System.Serializable]
public class PostProcessConfig
{
    public bool usePost = false;
    public bool disableInstead = false;
    public GameObject postOriginal;
    public GameObject postAlt;
    public float delay = 0f;
    public float duration = 2f;
    public TimerMode timerMode = TimerMode.Global;
    public EffectMode endMode = EffectMode.None;
}

[System.Serializable]
public class ShakeConfig
{
    public bool enabled = false;
    public float time = 0.5f;
    public float strength = 0.3f;
}

public class EnemyTriggerEvent : MonoBehaviour
{
    [Header("Geral")]
    public GameObject enemy;
    public GameObject flashlight;
    public ScreenShake screenShake;

    [Header("Duração do Evento")]
    public float eventDuration = 5f;
    public bool useCustomDuration = false;
    public float customDuration = 3f;

    [Header("Configs Gerais")]
    public LightConfig lightConfig;
    public EnemyConfig enemyConfig;
    public FlashlightConfig flashlightConfig;
    public PostProcessConfig postConfig;
    public ShakeConfig shakeConfig;

    private Renderer[] enemyRenderers;
    private NavMeshAgent enemyNav;
    private MonoBehaviour[] enemyScripts;
    private bool flashlightPrevState;

    private void Awake()
    {
        if (enemy != null)
        {
            enemyRenderers = enemy.GetComponentsInChildren<Renderer>(true);
            enemyNav = enemy.GetComponent<NavMeshAgent>();
            enemyScripts = enemy.GetComponents<MonoBehaviour>();
            SetEnemyVisible(false);
        }

        SaveLightState(lightConfig);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RunEvent());
            GetComponent<Collider>().enabled = false;
        }
    }

    private IEnumerator RunEvent()
    {
        float duration = useCustomDuration ? customDuration : eventDuration;

        if (flashlightConfig.disableTemporarily && flashlight != null)
            StartCoroutine(FlashlightOff());

        if (postConfig.usePost) StartCoroutine(HandlePost());

        if (shakeConfig.enabled && screenShake != null)
            screenShake.Shake(shakeConfig.time, shakeConfig.strength);

        if (lightConfig.lights.Length > 0)
            ApplyLightChanges(lightConfig);

        yield return new WaitForSeconds(enemyConfig.appearDelay);

        if (enemyConfig.enableEnemy && enemy != null)
        {
            if (enemyConfig.spawnPoint != null)
            {
                enemy.transform.position = enemyConfig.spawnPoint.position;
                enemy.transform.rotation = enemyConfig.spawnPoint.rotation;
            }
            SetEnemyVisible(true);
        }

        float waitTime = GetDurationForConfig(lightConfig.timerMode);
        yield return new WaitForSeconds(waitTime);

        if (enemyConfig.endMode != EffectMode.KeepEvent)
            SetEnemyVisible(false);

        HandleEndModes();
    }

    private float GetDurationForConfig(TimerMode mode)
    {
        return mode == TimerMode.Custom && useCustomDuration ? customDuration : eventDuration;
    }

    private void HandleEndModes()
    {
        // Luz
        if (lightConfig.endMode == EffectMode.None) RestoreLights(lightConfig);
        else if (lightConfig.endMode == EffectMode.Off) TurnLightsOff(lightConfig);

        // Lanterna
        if (flashlight != null)
        {
            switch (flashlightConfig.endMode)
            {
                case EffectModeLantern.Normal:
                    flashlight.SetActive(flashlightPrevState);
                    break;
                case EffectModeLantern.Off:
                    flashlight.SetActive(false);
                    break;
                case EffectModeLantern.On:
                    flashlight.SetActive(true);
                    break;
            }
        }

        // Inimigo
        if (enemyConfig.endMode == EffectMode.Off) SetEnemyVisible(false);

        // Pós-processamento
        if (postConfig.endMode == EffectMode.Off && postConfig.postOriginal != null) postConfig.postOriginal.SetActive(false);
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
        foreach (Light l in lightCfg.lights)
        {
            if (l == null) continue;
            if (lightCfg.changeColor) l.color = lightCfg.newColor;
            if (lightCfg.changeIntensity) l.intensity = lightCfg.newIntensity;
            if (lightCfg.changeRange) l.range = lightCfg.newRange;
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

        if (enemyScripts != null)
            foreach (MonoBehaviour script in enemyScripts)
                if (script != this) script.enabled = visible;
    }

    private IEnumerator FlashlightOff()
    {
        flashlightPrevState = flashlight.activeSelf;
        flashlight.SetActive(false);
        float waitTime = GetDurationForConfig(flashlightConfig.timerMode);
        yield return new WaitForSeconds(waitTime);
        flashlight.SetActive(flashlightPrevState);
    }

    private IEnumerator HandlePost()
    {
        if (postConfig.delay > 0f) yield return new WaitForSeconds(postConfig.delay);

        TogglePost(true);
        float waitTime = GetDurationForConfig(postConfig.timerMode);
        yield return new WaitForSeconds(waitTime);
        TogglePost(false);
    }

    private void TogglePost(bool alt)
    {
        if (postConfig.disableInstead)
        {
            if (postConfig.postOriginal != null)
                postConfig.postOriginal.SetActive(!alt);
        }
        else
        {
            if (postConfig.postOriginal != null) postConfig.postOriginal.SetActive(!alt);
            if (postConfig.postAlt != null) postConfig.postAlt.SetActive(alt);
        }
    }
}
