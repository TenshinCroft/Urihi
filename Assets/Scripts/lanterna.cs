using UnityEngine;

public class lanterna : MonoBehaviour
{
    //============== REFERÊNCIAS =================
    [Header("Referências")]
    public GameObject _lntrObj; // objeto da lanterna que vai ser ativado/desativado
    public GameObject _pObj;    // referência pro jogador

    //============== INTERNOS =====================
    private bool _lntrLigada = false; // armazena o último estado conhecido

    void Update()
    {
        // checa se o player tá atribuído
        if (_pObj == null || _lntrObj == null)
        {
            Debug.LogWarning("Referência não atribuída no script da lanterna!");
            return;
        }

        // tenta acessar a variável do script do Player
        var playerScript = _pObj.GetComponent<Player>();
        if (playerScript == null)
        {
            Debug.LogError("Script 'Player' não encontrado no objeto de player.");
            return;
        }

        // verifica o valor atual da variável booleana no Player
        bool estadoDesejado = playerScript._lntrOn;

        // se o estado mudou desde a última verificação
        if (_lntrLigada != estadoDesejado)
        {
            _lntrLigada = estadoDesejado;
            _lntrObj.SetActive(_lntrLigada); // ativa/desativa o objeto da lanterna
        }
    }
}
