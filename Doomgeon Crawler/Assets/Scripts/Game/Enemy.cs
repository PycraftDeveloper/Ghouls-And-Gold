using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Hurt SFX")]
    public List<AudioClip> HurtSounds;

    [Range(0, 1.0f)] public float HurtAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float HurtPitchRange;

    [Header("Death SFX")]
    public List<AudioClip> DeathSounds;

    [Range(0, 1.0f)] public float DeathAmplitude = 1.0f;
    [Range(0.0f, 0.15f)] public float DeathPitchRange;

    [Header("Melee Attack")]
    [SerializeField] private float MeleeDistance = 1.5f;

    [SerializeField] private float MeleeDamage = 5.0f;

    [SerializeField] private float MeleeCooldown = 0.5f; // seconds

    private float CurrentMeleeCooldown = -1;

    [Header("Line of Sight")]
    [SerializeField] private float ViewDistance = 50.0f;

    [SerializeField] private float FOV = 90.0f;

    [Header("Misc")]
    [SerializeField] private float Health = 20.0f;
    [SerializeField] private GameObject deadSprite;

    private NavMeshAgent playerAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerAgent = gameObject.GetComponent<NavMeshAgent>();
        playerAgent.updateRotation = true;
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentMeleeCooldown -= Time.deltaTime;

        if (CurrentMeleeCooldown <= 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, MeleeDistance);

            foreach (Collider col in hitColliders)
            {
                if (col.CompareTag("Player"))
                {
                    Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                    float angle = Vector3.Angle(transform.forward, directionToTarget);

                    col.GetComponent<Player>().DealDamage(MeleeDamage); // attacks on all sides
                }
            }
            CurrentMeleeCooldown = MeleeCooldown;
        }

        // Field of View --------------------------------------------------------------------
        Collider[] viewColliders = Physics.OverlapSphere(transform.position, ViewDistance);

        foreach (Collider col in viewColliders)
        {
            if (col.CompareTag("Player"))
            {
                Vector3 directionToTarget = (col.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToTarget);

                bool inFront = angle <= FOV / 2f;
                bool behind = angle >= 180f - (FOV / 2f);

                if (inFront || behind)
                {
                    playerAgent.destination = Registry.PlayerObject.transform.position;
                }
            }
        }
    }

    public void DealDamage(float Damage)
    {
        Health -= Damage;

        if (Health <= 0)
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    DeathSounds[Random.Range(0, DeathSounds.Count)],
                    Registry.SFX_Volume * DeathAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - DeathPitchRange, 1.0f + DeathPitchRange));

            Debug.Log("Enemy dead");
            Instantiate(deadSprite, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            Registry.CoreGameInfrastructureObject.Play_SFX_ExtendedOneShot(
                    HurtSounds[Random.Range(0, HurtSounds.Count)],
                    Registry.SFX_Volume * HurtAmplitude * Registry.Master_Volume,
                    0,
                    Random.Range(1.0f - HurtPitchRange, 1.0f + HurtPitchRange));

            Debug.Log("Enemy hurt");
        }
    }
}