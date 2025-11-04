using UnityEngine;

public class ChaseMusicController : MonoBehaviour
{
    [Header("Configura��o do �udio")]
    public AudioSource chaseMusic; // m�sica de persegui��o
    public bool canPlayMusic = false; // s� habilita ap�s evento 2

    private bool isPlaying = false;
    private bool forcedStart = false;

    public static ChaseMusicController instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StartChase()
    {
        if (!canPlayMusic) return;

        if (!isPlaying && chaseMusic != null)
        {
            chaseMusic.Play();
            isPlaying = true;
        }
    }

    public void StartChaseForced()
    {
        if (!canPlayMusic) return;

        forcedStart = true;
        if (!isPlaying && chaseMusic != null)
        {
            chaseMusic.Play();
            isPlaying = true;
        }
    }

    public void StopChase()
    {
        if (forcedStart) return;

        if (isPlaying && chaseMusic != null)
        {
            chaseMusic.Stop();
            isPlaying = false;
        }
    }

    public void StopChaseForced()
    {
        forcedStart = false;
        if (isPlaying && chaseMusic != null)
        {
            chaseMusic.Stop();
            isPlaying = false;
        }
    }

    public void EnableMusicAfterEvent2()
    {
        canPlayMusic = true;
    }
}
