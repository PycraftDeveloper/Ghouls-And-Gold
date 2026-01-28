using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Melee Attack")]
    [SerializeField] private float MeleeDistance = 1.5f;

    [SerializeField] private float MeleeDamage = 5.0f;

    //[SerializeField] private float MeleeAttackFOV = 45.0f; // degrees
    [SerializeField] private float MeleeCooldown = 0.5f; // seconds

    private float CurrentMeleeCooldown = -1;

    [Header("Misc")]
    [SerializeField] private float Health = 20.0f;

    private NavMeshAgent playerAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        playerAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    private void Update()
    {
        playerAgent.destination = Registry.PlayerObject.transform.position;
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

                    Debug.DrawRay(transform.position, directionToTarget, Color.red, 10.0f);
                }
            }
            CurrentMeleeCooldown = MeleeCooldown;
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