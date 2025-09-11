using UnityEngine;
using System.Collections;

public class porta : MonoBehaviour
{
    //====================== REFERÊNCIAS ======================
    [Header("Referências")]
    public Transform _Porta; // objeto da porta (visualmente)
    public int _itensParaAbrir;

    //====================== ESTADOS ======================
    [Header("Estados")]
    [HideInInspector]
    private bool _prtAbr = false; // se a porta está aberta
    private bool _prtAnim = false; // se a porta está em animação
    private bool _jaDestrancou = false; // se já destrancou
    public bool _podeAbrir = true;
    public bool _podeFechar = true;

    //====================== PARÂMETROS ======================
    [Header("Parâmetros")]
    public float _prtDur = 0.25f; // duração da animação

    //====================== VARIÁVEIS INTERNAS ======================
    private Quaternion _rotIni; // rotação inicial
    private Quaternion _rotAlv; // rotação alvo
    private float _tmpAnim;     // tempo atual da animação

    //====================== ÁUDIO ======================
    [Header("Áudio")]
    public AudioClip _somAbrir;
    public AudioClip _somFechar;
    public AudioClip _somDestrancar; // novo som de destrancar
    public bool _temDestrancar = false; // se essa porta tem som de destrancar
    private AudioSource _audioSource;

    //====================== START ======================
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    //====================== UPDATE ======================
    void Update()
    {
        if (_prtAnim)
        {
            _tmpAnim += Time.deltaTime;

            float _t = Mathf.Clamp01(_tmpAnim / _prtDur);

            _Porta.rotation = Quaternion.Slerp(_rotIni, _rotAlv, _t);

            if (_t >= 1f)
            {
                _prtAnim = false;
                _prtAbr = !_prtAbr;
            }
        }
    }

    //====================== ACIONAR PORTA ======================
    public void AcionarPorta()
    {
        // SE TEM SOM DE DESTRANCAR E AINDA NÃO DESTRANCOU
        if (_temDestrancar && !_jaDestrancou)
        {
            StartCoroutine(DestrancarEPermitirAbrir());
            return;
        }

        // ABRIR
        if (_podeAbrir && !_prtAbr)
        {
            if (_prtAnim) return;

            _prtAnim = true;
            _tmpAnim = 0f;
            _rotIni = _Porta.rotation;

            float _angY = 90f;
            _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);

            // Som de abertura
            if (_audioSource != null && _somAbrir != null)
            {
                _audioSource.PlayOneShot(_somAbrir);
            }
        }
        // FECHAR
        else if (_podeFechar && _prtAbr)
        {
            if (_prtAnim) return;

            _prtAnim = true;
            _tmpAnim = 0f;
            _rotIni = _Porta.rotation;

            float _angY = -90f;
            _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);

            // Som de fechamento
            if (_audioSource != null && _somFechar != null)
            {
                _audioSource.PlayOneShot(_somFechar);
            }
        }
    }

    //====================== CORROTINA: DESTRANCAR ======================
    IEnumerator DestrancarEPermitirAbrir()
    {
        if (_audioSource != null && _somDestrancar != null)
        {
            _audioSource.PlayOneShot(_somDestrancar);
            yield return new WaitForSeconds(_somDestrancar.length);
        }

        _jaDestrancou = true;

        // Acionar novamente para abrir após destrancar
        AcionarPorta();
    }
}

