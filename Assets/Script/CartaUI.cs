using UnityEngine;
using UnityEngine.UI;
public class CartaUI : MonoBehaviour
{
    [Header("UI da Carta")]
    public GameObject cartaPanel;   // Painel que contém a carta
    public Image cartaImage;        // Onde o sprite vai aparecer
    public Button fecharButton;     // Botão "X"

    void Start()
    {
        if (cartaPanel != null)
            cartaPanel.SetActive(false);

        if (fecharButton != null)
            fecharButton.onClick.AddListener(FecharCarta);
    }

    public void MostrarCarta(Sprite sprite)
    {
        if (cartaPanel != null)
        {
            cartaPanel.SetActive(true);
            if (cartaImage != null)
                cartaImage.sprite = sprite;

            Time.timeScale = 0f; // Congela jogo
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void FecharCarta()
    {
        if (cartaPanel != null)
        {
            cartaPanel.SetActive(false);
            Time.timeScale = 1f; // Descongela jogo
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
