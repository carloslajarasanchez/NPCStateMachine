using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyState { Patrolling, Chasing, Attacking }

    [Header("Configuración de Estados")]
    [SerializeField] private EnemyState _currentState = EnemyState.Patrolling;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _chaseTimeout = 10f;
    [SerializeField] private float _attackCooldown = 1.5f;

    [Header("Límites de Patrulla")]
    [SerializeField] private Vector3 _min, _max;


    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private Transform _playerTransform;

    private Vector3 _destination;
    private float _chaseTimer;
    private bool _playerInRange;
    private float _lastAttackTime;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        SetPatrolDestination();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Patrolling:
                UpdatePatrol();
                break;
            case EnemyState.Chasing:
                UpdateChase();
                break;
            case EnemyState.Attacking:
                UpdateAttack();
                break;
        }

        // Actualizamos siempre la velocidad del animador basada en el NavMeshAgent
        _animator.SetFloat("Velocity", _navMeshAgent.velocity.magnitude);
    }

    // --- LÓGICA DE PATRULLA ---
    private void UpdatePatrol()
    {
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            // Si llegó al punto, esperamos un poco y buscamos otro (podrías usar una corrutina aquí si quieres esperas largas)
            SetPatrolDestination();
        }

        if (_playerInRange)
        {
            TransitionToChase();
        }
    }

    private void SetPatrolDestination()
    {
        _destination = new Vector3(Random.Range(_min.x, _max.x), transform.position.y, Random.Range(_min.z, _max.z));
        _navMeshAgent.SetDestination(_destination);
    }

    // --- LÓGICA DE PERSECUCIÓN ---
    private void TransitionToChase()
    {
        _currentState = EnemyState.Chasing;
        _chaseTimer = 0f;
        Debug.Log("Persiguiendo al jugador...");
    }

    private void UpdateChase()
    {
        _chaseTimer += Time.deltaTime;

        if (_playerTransform != null)
        {
            _navMeshAgent.SetDestination(_playerTransform.position);

            float distance = Vector3.Distance(transform.position, _playerTransform.position);

            // Condición 1: Si está lo suficientemente cerca, ataca
            if (distance <= _attackRange)
            {
                _currentState = EnemyState.Attacking;
                return;
            }
        }

        // Condición 2: Si pasan 10 segundos y no lo ha alcanzado, vuelve a patrullar
        if (_chaseTimer >= _chaseTimeout)
        {
            Debug.Log("Tiempo agotado, volviendo a patrulla");
            _currentState = EnemyState.Patrolling;
            SetPatrolDestination();
        }
    }

    // --- LÓGICA DE ATAQUE ---
    private void UpdateAttack()
    {
        _navMeshAgent.isStopped = true;

        // Girar hacia el jugador
        if (_playerTransform != null)
        {
            Vector3 lookPos = _playerTransform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);

            // Lógica de daño con Cooldown
            if (Time.time >= _lastAttackTime + _attackCooldown)
            {
                _animator.SetTrigger("Attack");

                // Intentamos obtener el componente de salud del objeto detectado
                if (_playerTransform.TryGetComponent<PlayerHealth>(out var health))
                {
                    health.TakeDamage(1);
                }

                _lastAttackTime = Time.time;
            }

            // Si el jugador se aleja, volver a perseguir
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            if (distance > _attackRange)
            {
                _navMeshAgent.isStopped = false;
                _currentState = EnemyState.Chasing;
            }
        }
    }

    // --- DETECCIÓN POR TRIGGER ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
            _playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            // No reseteamos _playerTransform inmediatamente para evitar errores de null, 
            // pero el estado Chase se encargará de salir por tiempo si no lo ve.
        }
    }
}