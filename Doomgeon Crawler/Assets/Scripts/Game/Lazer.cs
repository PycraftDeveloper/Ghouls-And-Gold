using UnityEngine;

public class BossLaser : MonoBehaviour
{
    public float Speed = 25f;
    public float Lifetime = 2f;
    public float DamagePerSecond = 1f;
    public Vector3 Direction;

    private float lifeTimer;

    private void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= Lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Registry.PlayerObject.DealDamage(DamagePerSecond * Time.deltaTime, true);
        }
    }
}