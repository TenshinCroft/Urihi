using UnityEngine;

public class porta : MonoBehaviour
{
    //====================== REFER�NCIAS ======================
    [Header("Refer�ncias")]
    public Transform _prtObj; // objeto da porta (visualmente)
    public int _port;

    //====================== ESTADOS ======================
    [Header("Estados")]
    public bool _prtAbr = false; // se a porta est� aberta
    public bool _prtAnim = false; // se a porta est� em anima��o

    //====================== PAR�METROS ======================
    [Header("Par�metros")]
    public float _prtDur = 0.25f; // dura��o da anima��o

    //====================== VARI�VEIS INTERNAS ======================
    private Quaternion _rotIni; // rota��o inicial
    private Quaternion _rotAlv; // rota��o alvo
    private float _tmpAnim;     // tempo atual da anima��o

    //====================== UPDATE ======================
    void Update()
    {
        // verifica se t� animando
        if (_prtAnim)
        {
            // avan�a o tempo
            _tmpAnim += Time.deltaTime;

            // calcula o tempo em porcentagem da anima��o
            float _t = Mathf.Clamp01(_tmpAnim / _prtDur);

            // rotaciona suavemente entre inicial e alvo
            _prtObj.rotation = Quaternion.Slerp(_rotIni, _rotAlv, _t);

            // se chegou ao final
            if (_t >= 1f)
            {
                _prtAnim = false; // para anima��o
                _prtAbr = !_prtAbr; // troca o estado da porta
            }
        }
    }

    //====================== ACIONAR PORTA ======================
    public void AcionarPorta()
    {
        // se j� t� animando, ignora
        if (_prtAnim) return;

        // inicia anima��o
        _prtAnim = true;
        _tmpAnim = 0f;

        // salva rota��o atual
        _rotIni = _prtObj.rotation;

        // define rota��o alvo (abre ou fecha)
        float _angY = _prtAbr ? -90f : 90f;
        _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);
    }
}
