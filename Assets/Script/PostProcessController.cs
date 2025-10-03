using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [Header("Referências de Post Processing")]
    public GameObject postOriginal;
    public GameObject postAlt;

    private bool _isAlt = false;
    private MotionBlur motionBlurOriginal;
    private MotionBlur motionBlurAlt;

    private void Awake()
    {
        if (postOriginal != null) postOriginal.SetActive(true);
        if (postAlt != null) postAlt.SetActive(false);

        if (postOriginal != null)
        {
            Volume vol = postOriginal.GetComponent<Volume>();
            if (vol != null && vol.profile != null)
                vol.profile.TryGet(out motionBlurOriginal);
            
        }

        if (postAlt != null)
        {
            Volume vol = postAlt.GetComponent<Volume>();
            if (vol != null && vol.profile != null)
                vol.profile.TryGet(out motionBlurAlt);
        }
    }

    public void EnableMotionBlur(bool enabled)
    {
        if (motionBlurOriginal != null) motionBlurOriginal.active = enabled;
        if (motionBlurAlt != null) motionBlurAlt.active = enabled;
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
        EnableMotionBlur(false);
    }

    public bool IsAltActive() => _isAlt;
}
