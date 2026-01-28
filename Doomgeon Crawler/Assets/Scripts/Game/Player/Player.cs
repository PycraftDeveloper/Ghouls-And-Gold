using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
    [SerializeField] private float MeleeCooldown = 0.5f; // seconds
    private float CurrentMeleeCooldown = -1;

    [Header("Projectile Spell Attack")]
    [SerializeField] private GameObject ProjectilePrefab;

    [SerializeField] private float ProjectileSpellCooldown = 1.0f; // seconds
    private float CurrentProjectileSpellCooldown = -1;
    [SerializeField] private float ProjectileSpeed = 75.0f;
    [SerializeField] private float ProjectileDamage = 10.0f;
    [SerializeField] private float ManaCost = 1.0f;

    [Header("Inventory")]
    [SerializeField] private int GoldCount = 0;

    [SerializeField] private float ManaCount = 0;
    [SerializeField] private float HealthPickUpRegenAmount = 3.0f;

    [Header("Misc")]
    [SerializeField] private float Health = 20.0f;

    private GameObject LocalChest = null;

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

        inputActions.Player.SpellProjectile.performed += OnFireSpellProjectile;
        inputActions.Player.SpellProjectile.canceled += OnFireSpellProjectile;

        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Interact.canceled += OnInteract;
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

        inputActions.Player.SpellProjectile.performed -= OnFireSpellProjectile;
        inputActions.Player.SpellProjectile.canceled -= OnFireSpellProjectile;

        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Interact.canceled -= OnInteract;
    }

    private void Start()
    {
        Registry.PlayerObject = this;

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void Update()
    {
        PlayerCamera.transform.position = rb.position + CameraOffsetInPlayer;
        CurrentMeleeCooldown -= Time.deltaTime;
        CurrentProjectileSpellCooldown -= Time.deltaTime;
    }

    public void DealDamage(float Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            Debug.Log("Player dead");
        }
        else
        {
            Debug.Log("Player hurt");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
        else if (collision.gameObject.CompareTag("GoldPickUp"))
        {
            GoldCount++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("HealthPickUp"))
        {
            Health += HealthPickUpRegenAmount;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("ManaPickup"))
        {
            ManaCount++;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGrounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ChestInteractionArea"))
        {
            LocalChest = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ChestInteractionArea"))
        {
            LocalChest = null;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        MoveAxis = context.ReadValue<Vector2>();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (LocalChest != null)
        {
            LocalChest.GetComponent<Chest>().OnOpen();
        }
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
        if (CurrentMeleeCooldown <= 0)
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
            CurrentMeleeCooldown = MeleeCooldown;
        }
    }

    private void OnFireSpellProjectile(InputAction.CallbackContext context)
    {
        if (CurrentProjectileSpellCooldown <= 0 && ManaCount >= ManaCost)
        {
            ManaCount -= ManaCost;

            GameObject ProjectileInstance = Instantiate(ProjectilePrefab, transform.position + transform.forward, transform.rotation);
            ProjectileSpell instance = ProjectileInstance.GetComponent<ProjectileSpell>();
            instance.PositionDelta = PlayerCamera.transform.forward * ProjectileSpeed;
            instance.ProjectileDamage = ProjectileDamage;
            CurrentProjectileSpellCooldown = ProjectileSpellCooldown;
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