using UnityEngine;

public class canva : MonoBehaviour
{
    public GameObject win; // arrasta o canvas aqui no inspector
    public GameObject lose; // arrasta o canvas aqui no inspector
    public int contador = 0;

    public GameObject _player;
    public GameObject inimigo;

    public void Awake()
    {
        
    }
    void Update()
    {
        contador = _player.GetComponent<Player>()._itens;
        // Verifica se chegou no 7 e o canvas ainda t� escondido
        if (contador == 8 && !win.activeSelf)
        {
            win.SetActive(true);
            Debug.Log("Venceu");
        }
        if (inimigo.gameObject.GetComponent<Enemy>()._plyAtq && !lose.activeSelf)
        {
            lose.SetActive(true);
            Debug.Log("perdeu");
        }
    }
}
