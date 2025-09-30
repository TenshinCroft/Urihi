using UnityEngine;

public class ChaseMusicController : MonoBehaviour
{
    [Header("Configuração do Áudio")]
    public AudioSource chaseMusic; // música de perseguição
    public bool canPlayMusic = false; // só habilita após evento 2

    private bool isPlaying = false;

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

    public void StopChase()
    {
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
