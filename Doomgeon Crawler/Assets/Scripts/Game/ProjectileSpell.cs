using UnityEngine;

public class ProjectileSpell : MonoBehaviour
{
    [HideInInspector] public Vector3 PositionDelta;
    [HideInInspector] public float ProjectileDamage;
    public float Lifetime = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        GetComponent<Rigidbody>().linearVelocity = PositionDelta;
    }

    // Update is called once per frame
    private void Update()
    {
        Lifetime -= Time.deltaTime;

        if (Lifetime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            return;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().DealDamage(ProjectileDamage);
        }

        Destroy(gameObject);
    }
}