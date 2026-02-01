using UnityEngine;

public class TrapProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage = 5;
    [SerializeField] private float timeFromSpawn;
    private void Start()
    {
        timeFromSpawn = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().DealDamage(damage);
        }

        if (timeFromSpawn >= Time.time + 0.2f)
        {
            Destroy(gameObject);
        }
    }
}
