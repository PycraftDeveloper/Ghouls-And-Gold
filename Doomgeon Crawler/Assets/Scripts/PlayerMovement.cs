using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    private Rigidbody rb;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float moveSpeedMult;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private Vector3 CameraOffsetInPlayer = new Vector3(0, 1, 0);

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

    private void MovePlayer()
    {
        Vector3 velocity = new Vector3(
            MoveAxis.x * baseMoveSpeed * moveSpeedMult,
            rb.linearVelocity.y,
            MoveAxis.y * baseMoveSpeed * moveSpeedMult
        );

        rb.linearVelocity = velocity;
    }
}