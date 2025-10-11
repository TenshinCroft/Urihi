using UnityEngine;
using System.Collections;

public class porta : MonoBehaviour
{
    //====================== REFERÊNCIAS ======================
    [Header("Referências")]
    public Transform _Porta;
    public int _itensParaAbrir;

    //====================== ESTADOS ======================
    [Header("Estados")]
    [HideInInspector]
    public bool _isPortaAberta = false; // Estado para definir o alvo da rotação
    private bool _jaDestrancou = false;
    public bool _podeAbrir = true;
    public bool _podeFechar = true;

    //====================== PARÂMETROS ======================
    [Header("Parâmetros")]
    [Tooltip("Controla a velocidade da rotação (maior = mais rápido e mais responsivo à correção).")]
    public float _velocidadeSuave = 5f;

    [Header("Configuração de Ângulo")]
    [Tooltip("Ângulo de abertura da porta (ex: 90 ou -90).")]
    public float _anguloDeRotação = 90f;
    [Tooltip("Eixo de rotação da porta. Geralmente Vector3.up (Y).")]
    public Vector3 _eixoDeRotacao = Vector3.up;

    //====================== VARIÁVEIS INTERNAS FIXAS ======================
    private Quaternion _rotacaoFechada;
    private Quaternion _rotacaoAberta;

    //====================== ÁUDIO ======================
    [Header("Áudio")]
    public AudioClip _somAbrir;
    public AudioClip _somFechar;
    public AudioClip _somDestrancar;
    public AudioClip _somTrancada;
    public bool _temDestrancar = false;
    private AudioSource _audioSource;

    //====================== START ======================
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_Porta == null)
        {
            Debug.LogError("Referência _Porta não definida! Anexe a Transform da porta.");
            enabled = false; // Desativa o script se a referência estiver faltando
            return;
        }

        // 1. Define a rotação inicial FIXA (Porta Fechada)
        _rotacaoFechada = _Porta.localRotation;

        // 2. Calcula a rotação final FIXA (Porta Aberta)
        Quaternion angleOffset = Quaternion.AngleAxis(_anguloDeRotação, _eixoDeRotacao);
        _rotacaoAberta = _rotacaoFechada * angleOffset;
    }

    //====================== UPDATE - A CORREÇÃO ESTÁ AQUI ======================
    void Update()
    {
        // 1. Define o alvo de rotação com base no estado _isPortaAberta
        Quaternion targetRotation = _isPortaAberta ? _rotacaoAberta : _rotacaoFechada;

        // 2. Aplica a interpolação suave (Slerp) para corrigir a rotação
        _Porta.localRotation = Quaternion.Slerp(
            _Porta.localRotation,
            targetRotation,
            Time.deltaTime * _velocidadeSuave
        );
    }

    //====================== ACIONAR PORTA ======================
    public void AcionarPorta()
    {
        // Lógica de Destrancar
        if (_temDestrancar && !_jaDestrancou)
        {
            // OBS: A classe 'Player' não está no código. Assumindo que você a tem.
            Player player = FindObjectOfType<Player>();
            if (player != null && player._i >= _itensParaAbrir)
            {
                StartCoroutine(DestrancarEPermitirAbrir());
            }
            else
            {
                if (_audioSource != null && _somTrancada != null)
                {
                    _audioSource.PlayOneShot(_somTrancada);
                }
            }

            return;
        }

        // --- Lógica de Abrir/Fechar ---

        // Se a porta está fechada e pode abrir
        if (!_isPortaAberta && _podeAbrir)
        {
            _isPortaAberta = true;
            if (_audioSource != null && _somAbrir != null)
            {
                _audioSource.PlayOneShot(_somAbrir);
            }
        }
        // Se a porta está aberta e pode fechar
        else if (_isPortaAberta && _podeFechar)
        {
            _isPortaAberta = false;
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

        // Após destrancar, tenta abrir a porta
        AcionarPorta();
    }
}