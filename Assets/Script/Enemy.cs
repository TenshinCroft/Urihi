using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("�udio do Inimigo")]
    public AudioSource footstepAudio;
    public bool footstepsEnabled = false;
    private bool footstepsBlockedByEvent = false;
    //============== ANIMA��ES ===================
    public Animator anim;

    public string walkParam = "IsWalking";
    public string runParam = "IsRunning";
    public string attackParam = "Attack";

    [Header("Thresholds")]
    public float walkSpeedThreshold = 0.1f;
    public float runSpeedThreshold = 3.0f;

   


    //============== REFER�NCIAS =================
    [Header("Refer�ncias")]
    public GameObject _p;

    //============== STATUS ======================
    [Header("Status de Combate")]
    public bool _plyAtq = false;

    //============== MOVIMENTO ===================
    [Header("Par�metros de Movimento")]
    public float _stpDst = 1.7f;
    public float _chsRng = 15f;
    [Range(0, 360)]
    public float _fov = 135f;

    [Header("Velocidade")]
    public float _maxSpeed = 5f;       // velocidade m�xima
    public float _accelRate = 3f;      // acelera��o quando come�a a correr
    public float _decelRate = 6f;      // desacelera��o quando para
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

    
    public string attackStateName = "Attack";

    //============== INTERNOS ====================
    private NavMeshAgent _nav;
    public bool _playerVisible;
    public bool _IsPlayerInFOV;
    private bool _wasChasing = false;

    // hashes para performance
    private int walkHash;
    private int runHash;
    private int attackHash;
    private bool hasWalkParam = false;
    private bool hasRunParam = false;
    private bool hasAttackParam = false;

    private bool _attackInProgress = false;
    private bool _attackStateEntered = false;
    private float _attackFallbackTimer = 0f;
    private float _attackFallbackTimeout = 3f;


    public void Awake()
    {
        _nav = GetComponent<NavMeshAgent>();
        _nav.updateRotation = true;

        walkHash = Animator.StringToHash(walkParam ?? "");
        runHash = Animator.StringToHash(runParam ?? "");
        attackHash = Animator.StringToHash(attackParam ?? "");
        if (anim != null)
        {
            hasWalkParam = HasAnimatorParameter(anim, walkParam, AnimatorControllerParameterType.Bool);
            hasRunParam = HasAnimatorParameter(anim, runParam, AnimatorControllerParameterType.Bool);
            hasAttackParam = HasAnimatorParameter(anim, attackParam, AnimatorControllerParameterType.Trigger);

            if (!hasWalkParam)
                Debug.LogWarning($"Animator N�O possui o par�metro boolean '{walkParam}'. Verifique o nome no Animator (Parameters).");
            if (!hasRunParam)
                Debug.LogWarning($"Animator N�O possui o par�metro boolean '{runParam}'. Verifique o nome no Animator (Parameters).");
            if (!hasAttackParam)
                Debug.LogWarning($"Animator N�O possui o par�metro trigger '{attackParam}'. Verifique o nome no Animator (Parameters).");
        }
        else
        {
            Debug.LogWarning("Animator n�o atribu�do ou n�o encontrado no GameObject.");
        }

    }

    public void Start()
    {
        if (_p.transform == null)
            Debug.LogError("O jogador n�o foi atribu�do ao inimigo!");
        if (anim != null && anim.applyRootMotion)
            Debug.LogWarning("Recomendado: desmarcar 'Apply Root Motion' no Animator para que o NavMeshAgent controle a posi��o.");
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

        if (_playerVisible)
        {
            _nav.SetDestination(_p.transform.position);

            _nav.updateRotation = false;
            Vector3 dir = (_p.transform.position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);

            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxSpeed, _accelRate * Time.deltaTime);

            if (Vector3.Distance(_p.transform.position, transform.position) <= _stpDst)
                AttackPlayer();
        }
        else if (_waypoints != null && _waypoints.Length > 0)
        {
            _nav.updateRotation = true;
            Patrol();
            _curSpeed = Mathf.MoveTowards(_curSpeed, _maxSpeed * 0.5f, _accelRate * Time.deltaTime);
        }
        else
        {
            _nav.ResetPath();
            _curSpeed = Mathf.MoveTowards(_curSpeed, 0f, _decelRate * Time.deltaTime);
        }

        _nav.speed = _curSpeed;

        if (anim != null)
        {
            bool isWalking = _curSpeed > walkSpeedThreshold && _curSpeed <= runSpeedThreshold;
            bool isRunning = _curSpeed > runSpeedThreshold;

            if (hasWalkParam) anim.SetBool(walkHash, isWalking);
            if (hasRunParam) anim.SetBool(runHash, isRunning);
        }

        // Music control with state change detection
        if (_playerVisible && !_wasChasing)
        {
            Debug.Log($"Enemy detected player! Calling StartChase. Instance null? {ChaseMusicController.instance == null}");
            ChaseMusicController.instance?.StartChase();
            _wasChasing = true;
        }
        else if (!_playerVisible && _wasChasing)
        {
            Debug.Log($"Enemy lost player! Calling StopChase. Instance null? {ChaseMusicController.instance == null}");
            ChaseMusicController.instance?.StopChase();
            _wasChasing = false;
        }

        if (footstepsEnabled && !footstepsBlockedByEvent && footstepAudio != null && Time.timeScale > 0f)
        {
            bool isWalking = _curSpeed > walkSpeedThreshold && _curSpeed <= runSpeedThreshold;
            bool isRunning = _curSpeed > runSpeedThreshold;

            if ((isWalking || isRunning) && !footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
            else if (!isWalking && !isRunning && footstepAudio.isPlaying)
            {
                footstepAudio.Stop();
            }
        }
        else if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }

        if (_attackInProgress && anim != null)
        {
            AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

            if (!_attackStateEntered)
            {
                if (info.IsName(attackStateName))
                {
                    _attackStateEntered = true;
                    _attackFallbackTimer = 0f;
                }
                else
                {
                    _attackFallbackTimer += Time.deltaTime;
                    if (_attackFallbackTimer >= _attackFallbackTimeout) EndAttack();
                }
            }
            else
            {
                if (info.IsName(attackStateName) && info.normalizedTime >= 1f) EndAttack();
                else
                {
                    _attackFallbackTimer += Time.deltaTime;
                    if (_attackFallbackTimer >= _attackFallbackTimeout) EndAttack();
                }
            }
        }

    }

    private void EndAttack()
    {
        _plyAtq = false;
        _attackInProgress = false;
        _attackStateEntered = false;
        _attackFallbackTimer = 0f;
    }

    public void BlockFootsteps()
    {
        footstepsBlockedByEvent = true;
        if (footstepAudio != null && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    public void UnblockFootsteps()
    {
        footstepsBlockedByEvent = false;
    }





    public void AttackPlayer()
    {
        Debug.Log("Voc� Morreu");
        _plyAtq = true;
        _nav.ResetPath();
        _curSpeed = 0f; // trava movimento quando atacar

        if (anim != null && hasAttackParam)
        {
            anim.SetTrigger(attackHash);
            _attackInProgress = true;
            _attackStateEntered = false;
            _attackFallbackTimer = 0f;
        }
        else
        {
            if (anim == null) Debug.LogWarning("Sem Animator: ataque ser� apenas l�gico (sem anima��o).");
            else Debug.LogWarning($"Par�metro de ataque '{attackParam}' n�o encontrado no Animator; trigger ignorado.");
            _plyAtq = false;
        }
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
    // alternativa: chamar a partir de um Animation Event no final da anima��o de ataque
    public void OnAttackAnimationEnd() { EndAttack(); }
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
    private bool HasAnimatorParameter(Animator a, string paramName, AnimatorControllerParameterType type)
    {
        if (a == null || string.IsNullOrEmpty(paramName)) return false;
        foreach (var p in a.parameters)
        {
            if (p.name == paramName && p.type == type) return true;
        }
        return false;
    }
}
