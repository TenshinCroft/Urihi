using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [Header("Referências de Post Processing")]
    public GameObject postOriginal;   // Perfil normal
    public GameObject postAlt;        // Perfil alternativo

    private bool _isAlt = false;
    private MotionBlur motionBlur;

    private void Awake()
    {
        if (postOriginal != null) postOriginal.SetActive(true);
        if (postAlt != null) postAlt.SetActive(false);

        motionBlur = GetComponent<MotionBlur>(); // se tiver MotionBlur na Main Camera
    }

    public void SetOriginal()
    {
        _isAlt = false;
        if (postOriginal != null) postOriginal.SetActive(true);
        if (postAlt != null) postAlt.SetActive(false);
    }

    public void SetAlt(bool enableAlt)
    {
        _isAlt = enableAlt;
        if (postOriginal != null) postOriginal.SetActive(!enableAlt);
        if (postAlt != null) postAlt.SetActive(enableAlt);
    }

    public void SetOff()
    {
        _isAlt = false;
        if (postOriginal != null) postOriginal.SetActive(false);
        if (postAlt != null) postAlt.SetActive(false);
    }

    public bool IsAltActive()
    {
        return _isAlt;
    }

    public void EnableMotionBlur(bool enabled)
    {
        if (motionBlur != null) motionBlur.enabled = enabled;
    }
}
