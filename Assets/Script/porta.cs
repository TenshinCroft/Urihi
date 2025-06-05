using UnityEngine;

public class porta : MonoBehaviour
{
    //====================== ESTADOS ======================
    [Header("Estado da Porta")]
    public bool _prtAbr = false;

    //====================== REFERÊNCIAS ======================
    [Header("Referências")]
    public GameObject _prtObj; // objeto visual da porta

    //====================== AWAKE ======================
    public void Awake()
    {
        // inicia com a porta fechada
        _prtAbr = false;
    }

    //====================== ABRIR / FECHAR ======================
    // função que alterna entre abrir e fechar a porta
    public void AbrirPorta()
    {
        // se estiver fechada, abre
        if (!_prtAbr)
        {
            // gira o objeto da porta pra abrir (90° no eixo Y)
            _prtObj.transform.Rotate(0f, 90f, 0f);
            _prtAbr = true;
        }
        // se já estiver aberta, fecha
        else
        {
            // gira de volta (fecha a porta)
            _prtObj.transform.Rotate(0f, -90f, 0f);
            _prtAbr = false;
        }
    }
}
