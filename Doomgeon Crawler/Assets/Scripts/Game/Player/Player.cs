using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Hurt SFX")]
    public List<AudioClip> HurtSounds;

    [Range(0, 1.0f)] public float HurtAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float HurtPitchRange;

    [Header("Death SFX")]
    public List<AudioClip> DeathSounds;

    [Range(0, 1.0f)] public float DeathAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float DeathPitchRange;

    [Header("Gold Pickup SFX")]
    public List<AudioClip> GoldPickupSounds;

    [Range(0, 1.0f)] public float GoldPickUpAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float GoldPickUpPitchRange;

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

    [Header("User Interface")]
    [SerializeField] private Slider healthBar;

    [SerializeField] private Slider manaBar;

    private GameObject LocalChest = null;
    private GameObject LocalLever = null;

    private PlayerInput inputActions;
    private Vector2 MoveAxis;
    private bool IsGrounded = false;
    private bool Dead = false; // prevent repeated death sounds

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
            if (!Dead)
            {
                Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                        DeathSounds[Random.Range(0, DeathSounds.Count)],
                        Registry.SFX_Volume * DeathAmplitude * Registry.Master_Volume,
                        0,
                        Random.Range(1.0f - DeathPitchRange, 1.0f + DeathPitchRange));

                Debug.Log("Player dead");

                Dead = true;
            }
        }
        else
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    HurtSounds[Random.Range(0, HurtSounds.Count)],
                    Registry.SFX_Volume * HurtAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - HurtPitchRange, 1.0f + HurtPitchRange));

            Debug.Log("Player hurt");
        }

        UpdateUserInterface();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ChestInteractionArea"))
        {
            LocalChest = other.gameObject;
        }
        else if (other.gameObject.CompareTag("GoldPickUp"))
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    GoldPickupSounds[Random.Range(0, GoldPickupSounds.Count)],
                    Registry.SFX_Volume * GoldPickUpAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - GoldPickUpPitchRange, 1.0f + GoldPickUpPitchRange));

            GoldCount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("HealthPickUp"))
        {
            Health += HealthPickUpRegenAmount;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("ManaPickup"))
        {
            ManaCount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("Lever"))
        {
            LocalLever = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("ChestInteractionArea"))
        {
            LocalChest = null;
        }
        else if (other.gameObject.CompareTag("Lever"))
        {
            LocalLever = null;
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

        if (LocalLever != null)
        {
            Debug.Log("InteractedWithLever");
            LocalLever.GetComponent<Lever>().ChangeLeverState();
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

            GameObject ProjectileInstance = Instantiate(ProjectilePrefab, transform.position + -PlayerCamera.transform.right, transform.rotation);
            ProjectileSpell instance = ProjectileInstance.GetComponent<ProjectileSpell>();
            instance.PositionDelta = PlayerCamera.transform.forward * ProjectileSpeed;
            instance.ProjectileDamage = ProjectileDamage;
            CurrentProjectileSpellCooldown = ProjectileSpellCooldown;

            UpdateUserInterface();
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

    private void UpdateUserInterface()
    {
        healthBar.value = Health;
        manaBar.value = ManaCount;
    }
}