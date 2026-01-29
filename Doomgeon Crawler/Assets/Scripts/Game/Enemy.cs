using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
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
            Debug.Log("Enemy dead");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Enemy hurt");
        }
    }
}