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
    public bool _playerVisible;
    public bool _IsPlayerInFOV;

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
        IsPlayerInFOV();

        if (_IsPlayerInFOV && !_p.GetComponent<goToPlayer>()._inCls)
        {
            _playerVisible = true;
            _lostTimer = 0f; // zera o tempo perdido
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
    public void IsPlayerInFOV()
    {
        // Assumindo que player é Transform
        Vector3 directionToPlayer = _p.transform.position - transform.position;
        float distanceToPlayer = Vector3.Distance(_p.transform.position, transform.position);

        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, distanceToPlayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Verifica se tá dentro do range E do campo de visão
                float angle = Vector3.Angle(transform.forward, directionToPlayer);
                if (distanceToPlayer <= _chsRng && angle <= _fov / 2f)
                {
                    _IsPlayerInFOV = true;
                    return;
                }

            }
            else
            {
                // Tem algo na frente (parede, objeto, etc)
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

            // Desenha o raio de visão direta
            Gizmos.color = Color.green;
            Vector3 from = transform.position + Vector3.up * 1.5f;
            Vector3 to = _p.transform.position + Vector3.up * 1.5f;
            Gizmos.DrawLine(from, to);
        }
    }
}