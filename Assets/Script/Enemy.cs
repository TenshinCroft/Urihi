using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //============== REFERÊNCIAS =================
    [Header("Referências")]
    public GameObject _p;

    //============== STATUS ======================
    [Header("Status de Combate")]
    public bool _plyAtq = false;

    //============== MOVIMENTO ===================
    [Header("Parâmetros de Movimento")]
    public float _stpDst = 2f;
    public float _chsRng = 15f;
    [Range(0, 360)]
    public float _fov = 135f;

    //============== IA ==========================
    [Header("Timers")]
    public float _maxLostTime = 2f; // tipo 2 segundos de memória
    private float _lostTimer = 0f;

    public LayerMask _visionMask; // <-- adiciona no topo do script pra setar pelo Inspector

    [Header("Sondagem")]
    public Transform[] _waypoints; // pontos que o inimigo vai patrulhar
    private int _curWp = 0;
    public bool _giz;

    //============== INTERNOS ====================
    private NavMeshAgent _nav;
    private bool _playerVisible;

    public void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
    }

    public void Start()
    {
        if (_p.transform == null)
            Debug.LogError("O jogador não foi atribuído ao inimigo!");
    }

    public void Update()
    {
        Vector3 from = transform.position;
        Vector3 to = _p.transform.position;
        Vector3 dir = to - from;

        Debug.DrawRay(from, dir, Color.green, 5f);
        float _dstToPly = Vector3.Distance(transform.position, _p.transform.position);

        if (IsPlayerInFOV())
        {
            _playerVisible = true;
            _lostTimer = 0f; // zera o tempo perdido
        }
        else
        {
            _lostTimer += Time.deltaTime;
            if(!_p.GetComponent<goToPlayer>()._inCls)
            {
                _playerVisible = false;
            }

            if (_lostTimer >= _maxLostTime)
            {
                _playerVisible = false; // depois do delay, esquece
            }
        }

        if (_playerVisible)
        {
            _nav.SetDestination(_p.transform.position);
        }
        else if (_waypoints != null)
        {
            Patrol();
        }
        else
        {
            _nav.ResetPath();
        }
    }



    public void ChasePlayer()
    {
        _nav.SetDestination(_p.transform.position);
    }


    public void ChasePlayer(float _dstToPly)
    {
        if (_dstToPly > _stpDst)
        {
            _nav.SetDestination(_p.transform.position);
        }
        else
        {
            AttackPlayer();
        }
    }

    public void AttackPlayer()
    {
        Debug.Log("Você Morreu");
        _plyAtq = true;
        _nav.ResetPath();
    }

    public void Patrol()
    {
        if (_waypoints.Length == 0) return;

        if (!_nav.hasPath || _nav.remainingDistance < 1f)
        {
            _nav.SetDestination(_waypoints[_curWp].position);
            _curWp = (_curWp + 1) % _waypoints.Length;
        }
    }
    public bool IsPlayerInFOV()
    {
        Vector3 _dirToPlayer = (_p.transform.position - transform.position).normalized;
        float _angle = Vector3.Angle(transform.forward, _dirToPlayer);

        return _angle <= _fov / 2f && Vector3.Distance(transform.position, _p.transform.position) <= _chsRng;
    }
    public void OnDrawGizmos()
    {
        if (_giz)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _stpDst);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chsRng);

            Gizmos.color = Color.red;
            Vector3 _lftRay = Quaternion.Euler(0, -_fov / 2f, 0) * transform.forward;
            Vector3 _rgtRay = Quaternion.Euler(0, _fov / 2f, 0) * transform.forward;
            Gizmos.DrawRay(transform.position, _lftRay * _chsRng);
            Gizmos.DrawRay(transform.position, _rgtRay * _chsRng);

            // Desenha o raio de visão direta
            Gizmos.color = Color.green;
            Vector3 from = transform.position + Vector3.up * 1.5f;
            Vector3 to = _p.transform.position + Vector3.up * 1.5f;
            Gizmos.DrawLine(from, to);
        }
    }
}