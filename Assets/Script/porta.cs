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
    private bool _prtAbr = false; 
    private bool _prtAnim = false; 
    private bool _jaDestrancou = false; 
    public bool _podeAbrir = true;
    public bool _podeFechar = true;

    //====================== PARÂMETROS ======================
    [Header("Parâmetros")]
    public float _prtDur = 0.25f; 

    //====================== VARIÁVEIS INTERNAS ======================
    private Quaternion _rotIni; 
    private Quaternion _rotAlv; 
    private float _tmpAnim;    

    //====================== ÁUDIO ======================
    [Header("Áudio")]
    public AudioClip _somAbrir;
    public AudioClip _somFechar;
    public AudioClip _somDestrancar; 
    public bool _temDestrancar = false; 
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
        
        if (_temDestrancar && !_jaDestrancou)
        {
            StartCoroutine(DestrancarEPermitirAbrir());
            return;
        }

       
        if (_podeAbrir && !_prtAbr)
        {
            if (_prtAnim) return;

            _prtAnim = true;
            _tmpAnim = 0f;
            _rotIni = _Porta.rotation;

            float _angY = 90f;
            _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);

            
            if (_audioSource != null && _somAbrir != null)
            {
                _audioSource.PlayOneShot(_somAbrir);
            }
        }
        
        else if (_podeFechar && _prtAbr)
        {
            if (_prtAnim) return;

            _prtAnim = true;
            _tmpAnim = 0f;
            _rotIni = _Porta.rotation;

            float _angY = -90f;
            _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);

        
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

      
        AcionarPorta();
    }
}

