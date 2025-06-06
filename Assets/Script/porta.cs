using UnityEngine;

public class porta : MonoBehaviour
{
    //====================== REFERÊNCIAS ======================
    [Header("Referências")]
    public Transform _prtObj; // objeto da porta (visualmente)
    public int _port;

    //====================== ESTADOS ======================
    [Header("Estados")]
    public bool _prtAbr = false; // se a porta está aberta
    public bool _prtAnim = false; // se a porta está em animação

    //====================== PARÂMETROS ======================
    [Header("Parâmetros")]
    public float _prtDur = 0.25f; // duração da animação

    //====================== VARIÁVEIS INTERNAS ======================
    private Quaternion _rotIni; // rotação inicial
    private Quaternion _rotAlv; // rotação alvo
    private float _tmpAnim;     // tempo atual da animação

    //====================== UPDATE ======================
    void Update()
    {
        // verifica se tá animando
        if (_prtAnim)
        {
            // avança o tempo
            _tmpAnim += Time.deltaTime;

            // calcula o tempo em porcentagem da animação
            float _t = Mathf.Clamp01(_tmpAnim / _prtDur);

            // rotaciona suavemente entre inicial e alvo
            _prtObj.rotation = Quaternion.Slerp(_rotIni, _rotAlv, _t);

            // se chegou ao final
            if (_t >= 1f)
            {
                _prtAnim = false; // para animação
                _prtAbr = !_prtAbr; // troca o estado da porta
            }
        }
    }

    //====================== ACIONAR PORTA ======================
    public void AcionarPorta()
    {
        // se já tá animando, ignora
        if (_prtAnim) return;

        // inicia animação
        _prtAnim = true;
        _tmpAnim = 0f;

        // salva rotação atual
        _rotIni = _prtObj.rotation;

        // define rotação alvo (abre ou fecha)
        float _angY = _prtAbr ? -90f : 90f;
        _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);
    }
}
