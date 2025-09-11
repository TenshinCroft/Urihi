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
    public float _stpDst = 1.7f;
    public float _chsRng = 15f;
    [Range(0, 360)]
    public float _fov = 135f;

    [Header("Velocidade")]
    public float _maxSpeed = 5f;       // velocidade máxima
    public float _accelRate = 3f;      // aceleração quando começa a correr
    public float _decelRate = 6f;      // desaceleração quando para
    private float _curSpeed = 0f;      // velocidade atual

    //============== IA ==========================
    [Header("Timers")]
    public float _maxLostTime = 2f;
    private float _lostTimer = 0f;

    public LayerMask _visionMask;

    [Header("Sondagem")]
    public Transform[] _waypoints;
    private int _curWp = 0;
    public bool _giz;

    //============== INTERNOS ====================
    private NavMeshAgent _nav;
    public bool _playerVisible;
    public bool _IsPlayerInFOV;

    public void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = true;
    }

    public void Start()
    {
        if (_p.transform == null)
            Debug.LogError("O jogador não foi atribuído ao inimigo!");
    }

    public void Update()
    {
        IsPlayerInFOV();

        if (_IsPlayerInFOV && !_p.GetComponent<goToPlayer>()._inCls)
        {
            _playerVisible = true;
            _lostTimer = 0f;
        }
        else
        {
            _lostTimer += Time.deltaTime;
            if (_p.GetComponent<goToPlayer>()._inCls)
            {
                _playerVisible = false;
            }
            else if (_lostTimer >= _maxLostTime)
            {
                _playerVisible = false;
            }
        }

        // define destino + controle de aceleração
        if (_playerVisible)
        {
            _nav.SetDestination(_p.transform.position);

            // rotação instantânea pro player
            _nav.updateRotation = false;
            Vector3 dir = (_p.transform.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            // acelera até a velocidade máxima
            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxSpeed, _accelRate * Time.deltaTime);

            if (Vector3.Distance(_p.transform.position, transform.position) <= _stpDst)
                AttackPlayer();
        }
        else if (_waypoints != null && _waypoints.Length > 0)
        {
            _nav.updateRotation = true;
            Patrol();
            // desacelera um pouco quando patrulhando
            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxSpeed * 0.5f, _accelRate * Time.deltaTime);
        }
        else
        {
            _nav.ResetPath();
            // desaceleração rápida
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0f, _decelRate * Time.deltaTime);
        }

        // aplica a velocidade no navmesh
        _nav.speed = _curSpeed;
    }

    public void AttackPlayer()
    {
        Debug.Log("Você Morreu");
        _plyAtq = true;
        _nav.ResetPath();
        _curSpeed = 0f; // trava movimento quando atacar
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

    public void IsPlayerInFOV()
    {
        Vector3 directionToPlayer = _p.transform.position - transform.position;
        float distanceToPlayer = Vector3.Distance(_p.transform.position, transform.position);

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                if (distanceToPlayer <= _chsRng && angle <= _fov / 2f)
                {
                    _IsPlayerInFOV = true;
                    return;
                }
            }
            else
            {
                _IsPlayerInFOV = false;
            }
        }
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

            Gizmos.color = Color.green;
            Vector3 from = transform.position + Vector3.up * 1.5f;
            Vector3 to = _p.transform.position + Vector3.up * 1.5f;
            Gizmos.DrawLine(from, to);
        }
    }
}
