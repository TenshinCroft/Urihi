using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //============== REFERÊNCIAS =================
    [Header("Referências")]
    public Transform _plyTrf;

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
    public float _maxLostTime = 10f; // tempo pra entrar no modo de sondagem
    private float _lostTimer = 0f; // contador interno

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
        if (_plyTrf == null)
            Debug.LogError("O jogador não foi atribuído ao inimigo!");
    }

    public void Update()
    {
        float _dstToPly = Vector3.Distance(transform.position, _plyTrf.position);
        _playerVisible = _dstToPly <= _chsRng && IsPlayerInFOV();

        if (_playerVisible)
        {
            _lostTimer = 0f; // reseta o timer
            ChasePlayer(_dstToPly);
        }
        else
        {
            _lostTimer += Time.deltaTime;

            if (_lostTimer >= _maxLostTime)
            {
                Patrol(); // entra no modo de sondagem
            }
        }
    }

    public void ChasePlayer(float _dstToPly)
    {
        if (_dstToPly > _stpDst)
        {
            _nav.SetDestination(_plyTrf.position);
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
        Vector3 _dirToPlayer = (_plyTrf.position - transform.position).normalized;
        float _angle = Vector3.Angle(transform.forward, _dirToPlayer);
        return _angle <= _fov / 2f;
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
        }
    }
}
