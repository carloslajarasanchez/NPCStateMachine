using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private CharacterController _characterController;
    private Vector2 _inputMovement;
    private Vector3 _direction;
    private Animator _animator;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Activamos el mapa de controles de "Player" al iniciar
        PlayerInputManager.SwitchControlMap(PlayerInputManager.ControlMap.Player);
    }

    private void Update()
    {
        ReadInput();
        ApplyRotation();
        ApplyMovement();
        UpdateAnimations();
    }

    private void ReadInput()
    {
        // Leemos el Vector2 directamente de tu Singleton
        _inputMovement = PlayerInputManager.Actions.Player.Move.ReadValue<Vector2>();

        // Convertimos el input 2D (x, y) a una dirección 3D (x, 0, z)
        _direction = new Vector3(_inputMovement.x, 0f, _inputMovement.y).normalized;
    }

    private void ApplyMovement()
    {
        // Movimiento simple usando CharacterController
        if (_direction.magnitude >= 0.1f)
        {
            _characterController.Move(_direction * moveSpeed * Time.deltaTime);
        }

        // Gravedad básica para que no flote si hay desniveles
        if (!_characterController.isGrounded)
        {
            _characterController.Move(Vector3.down * 9.81f * Time.deltaTime);
        }
    }

    private void ApplyRotation()
    {
        // Si hay movimiento, rotar hacia la dirección de la marcha
        if (_direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void UpdateAnimations()
    {
        _animator.SetFloat("Velocity", _direction.magnitude);
    }
}