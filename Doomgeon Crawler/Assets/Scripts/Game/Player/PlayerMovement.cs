using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Player Movement")]
    [SerializeField] private float baseMoveSpeed;

    [SerializeField] private float moveSpeedMult;
    [SerializeField] private float jumpForce = 6f;

    [Header("Player Camera")]
    [SerializeField] private Camera PlayerCamera;

    [SerializeField] private Vector3 CameraOffsetInPlayer = new Vector3(0, 1, 0);

    [Header("Melee Attack")]
    [SerializeField] private float MeleeDistance = 3.0f;

    [SerializeField] private float MeleeDamage = 5.0f;
    [SerializeField] private float MeleeAttackFOV = 45.0f; // degrees

    private PlayerInput inputActions;
    private Vector2 MoveAxis;
    private bool IsGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new PlayerInput();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

        inputActions.Player.Attack.performed += OnAttack;
        inputActions.Player.Attack.canceled += OnAttack;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

        inputActions.Player.Attack.performed -= OnAttack;
        inputActions.Player.Attack.canceled -= OnAttack;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        PlayerCamera.transform.position = rb.position + CameraOffsetInPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveAxis = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (IsGrounded)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                jumpForce,
                rb.linearVelocity.z
            );
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, MeleeDistance);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(PlayerCamera.transform.forward, directionToTarget);

                if (angle <= MeleeAttackFOV / 2f)
                {
                    col.GetComponent<Enemy>().DealDamage(MeleeDamage);
                }
            }
        }
    }

    private void MovePlayer()
    {
        Vector3 velocity = PlayerCamera.transform.rotation * new Vector3(
            MoveAxis.x * baseMoveSpeed * moveSpeedMult,
            0,
            MoveAxis.y * baseMoveSpeed * moveSpeedMult
        );

        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }
}