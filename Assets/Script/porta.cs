using UnityEngine;
using System.Collections;

public class porta : MonoBehaviour
{

        //====================== REFERÊNCIAS ======================
        [Header("Referências")]
        public Transform _Porta;
        public int _itensParaAbrir; // O número necessário de itens (chaves, etc.)
                                    // Adicione uma referência ao inventário ou script que gerencia os itens.
                                    // Exemplo:
                                    // public Inventario _inventario; 

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
        // NOVO: AudioClip para o som de porta trancada
        public AudioClip _somTrancada;
        private AudioSource _audioSource;

        //====================== START ======================
        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            // É bom garantir que a porta trancada só possa ser destrancada se tiver um requisito.
            if (_itensParaAbrir > 0)
            {
                _temDestrancar = true;
            }
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

        //====================== FUNÇÃO DE VERIFICAÇÃO DE ITENS ======================
        // Esta função deve retornar o número de itens que o jogador possui.
        // Você deve adaptá-la para o seu sistema de inventário.
        private int VerificarItensNecessarios()
        {
            // **SUBSTITUA ESTE CÓDIGO** pela lógica real do seu jogo.
            // Por exemplo, se você tem um script de Inventario com uma função GetItemCount("Chave"):
            // if (_inventario != null)
            // {
            //     return _inventario.GetItemCount("Chave"); // Exemplo
            // }
            // Por enquanto, vamos retornar 0 para simular a falta do item.
            return 0; // Altere isto!
        }

        //====================== ACIONAR PORTA ======================
        public void AcionarPorta()
        {
            // 1. Lógica de porta trancada/destrancada
            if (_temDestrancar && !_jaDestrancou)
            {
                // Verifica se o jogador tem os itens necessários
                if (VerificarItensNecessarios() >= _itensParaAbrir)
                {
                    // Se tiver os itens, inicia a corrotina para destrancar e abrir
                    StartCoroutine(DestrancarEPermitirAbrir());
                    return;
                }
                else
                {
                    // NOVO: Toca o som de porta trancada
                    if (_audioSource != null && _somTrancada != null)
                    {
                        _audioSource.PlayOneShot(_somTrancada);
                        // Opcional: Adicionar feedback visual ou de texto aqui (ex: "Trancada!")
                    }
                    return; // Impede a continuação e abertura
                }
            }

            // 2. Lógica de Abertura (se estiver destrancada ou não exigir itens)
            if (_podeAbrir && !_prtAbr)
            {
                if (_prtAnim) return;

                _prtAnim = true;
                _tmpAnim = 0f;
                _rotIni = _Porta.rotation;

                // Assumindo que a porta abre 90 graus no eixo Y (ajuste conforme necessário)
                float _angY = 90f;
                _rotAlv = _rotIni * Quaternion.Euler(0f, _angY, 0f);

                if (_audioSource != null && _somAbrir != null)
                {
                    _audioSource.PlayOneShot(_somAbrir);
                }
            }
            // 3. Lógica de Fechamento
            else if (_podeFechar && _prtAbr)
            {
                if (_prtAnim) return;

                _prtAnim = true;
                _tmpAnim = 0f;
                _rotIni = _Porta.rotation;

                // Assumindo que a porta fecha -90 graus no eixo Y (ajuste conforme necessário)
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

            // Tenta abrir a porta logo após destrancar
            AcionarPorta();
        }
    }


