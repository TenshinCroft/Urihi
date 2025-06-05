using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //================== REFERÊNCIAS ==================
    [Header("Referências")]
    public GameObject _p; // referência do player

    //================== STATUS DE COMBATE ==================
    [Header("Status de Combate")]
    public bool _pAtq = false; // se o inimigo está atacando

    //================== MOVIMENTO ==================
    [Header("Parâmetros de Movimento")]
    public float _stpDst = 2f; // distância de parada (pra atacar)
    public float _chsRng = 15f; // distância máxima pra perseguir
    [Range(0, 360)] public float _fov = 135f; // campo de visão

    //================== TEMPORIZADORES ==================
    [Header("Timers")]
    public float _maxLost = 10f; // tempo pra desistir e patrulhar
    private float _lostT = 0f; // timer interno

    public float _memDur = 2f; // memória visual: tempo que "lembra" do player
    private float _memT = 0f;

    //================== PATRULHA ==================
    [Header("Sondagem")]
    public Transform[] _wps; // pontos de patrulha
    private int _wpInd = 0; // index do waypoint atual
    public bool _giz = true; // mostrar gizmos?

    //================== INTERNOS ==================
    private NavMeshAgent _nav; // agente de navmesh
    private bool _pVisible; // se o player tá visível

    //================== AWAKE ==================
    public void Awake()
    {
        // pega o navmesh agent
        _nav = GetComponent<NavMeshAgent>();
    }

    //================== START ==================
    public void Start()
    {
        if (_p == null)
        {
            Debug.LogError("Player não atribuído no inimigo!");
        }
    }

    //================== UPDATE ==================
    public void Update()
    {
        // mede a distância até o player
        float _dist = Vector3.Distance(transform.position, _p.transform.position);

        // checa se tá no campo de visão
        if (IsPlayerInFOV())
        {
            _pVisible = true;
            _memT = 0f; // zera o tempo de "memória"
        }
        else
        {
            _memT += Time.deltaTime;
            if (_memT >= _memDur)
            {
                _pVisible = false; // esquece do player
            }
        }

        // se vê o player, persegue
        if (_pVisible)
        {
            _nav.SetDestination(_p.transform.position);
        }
        else
        {
            _nav.ResetPath(); // para de andar
        }
    }

    //================== PERSEGUIÇÃO ==================
    public void ChasePlayer()
    {
        _nav.SetDestination(_p.transform.position);
    }

    //================== ATAQUE ==================
    public void AttackPlayer()
    {
        Debug.Log("Você Morreu");
        _pAtq = true;
        _nav.ResetPath(); // para ao atacar
    }

    //================== PATRULHA ==================
    public void Patrol()
    {
        if (_wps.Length == 0) return;

        // se chegou no waypoint, vai pro próximo
        if (!_nav.hasPath || _nav.remainingDistance < 1f)
        {
            _nav.SetDestination(_wps[_wpInd].position);
            _wpInd = (_wpInd + 1) % _wps.Length;
        }
    }

    //================== VISÃO ==================
    public bool IsPlayerInFOV()
    {
        // se o player estiver escondido, ignora
        var _plyScript = _p.GetComponent<goToPlayer>();
        if (_plyScript != null && _plyScript._inCls) return false;

        // direção até o player
        Vector3 _dir = (_p.transform.position - transform.position).normalized;
        float _ang = Vector3.Angle(transform.forward, _dir);

        // se tá dentro do campo de visão
        if (_ang <= _fov / 2f)
        {
            // altura dos olhos
            Vector3 _orig = transform.position + Vector3.up * 1.5f;
            Vector3 _target = _p.transform.position + Vector3.up * 1.5f;
            Vector3 _rayDir = _target - _orig;
            float _rayDist = _rayDir.magnitude;

            RaycastHit _hit;
            // faz o raycast pra ver se tem visão direta
            if (Physics.Raycast(_orig, _rayDir.normalized, out _hit, _rayDist))
            {
                if (_hit.transform == _p.transform)
                {
                    return true; // visão direta
                }
            }
        }

        return false; // fora do campo ou sem visão direta
    }

    //================== GIZMOS ==================
    public void OnDrawGizmos()
    {
        if (!_giz) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _stpDst); // distância de ataque

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _chsRng); // alcance de perseguição

        // linhas do campo de visão
        Gizmos.color = Color.red;
        Vector3 _lf = Quaternion.Euler(0, -_fov / 2f, 0) * transform.forward;
        Vector3 _rg = Quaternion.Euler(0, _fov / 2f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, _lf * _chsRng);
        Gizmos.DrawRay(transform.position, _rg * _chsRng);

        // linha direta até o player
        if (_p != null)
        {
            Gizmos.color = Color.green;
            Vector3 _from = transform.position + Vector3.up * 1.5f;
            Vector3 _to = _p.transform.position + Vector3.up * 1.5f;
            Gizmos.DrawLine(_from, _to);
        }
    }
}
