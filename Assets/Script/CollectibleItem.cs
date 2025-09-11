using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
<<<<<<< Updated upstream
    public string itemName = "Item";
    public AudioClip collectSound;
=======
    public string itemName = "Carta";
    public GameObject _cartaImage;
    public bool _carta = false;
    public void Update()
    {
        if (_cartaImage != null)
        {
            _cartaImage.SetActive(_carta);
        }
    }
>>>>>>> Stashed changes

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Item coletado: " + itemName);

            if (collectSound != null)
            {
                // Cria um objeto temporário para o som
                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = transform.position;

                AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                audioSource.Play();

                // Destroi o objeto de som após o tempo do clip
                Destroy(tempAudio, collectSound.length);
            }

            Destroy(gameObject); // destrói o item
        }
    }
}
