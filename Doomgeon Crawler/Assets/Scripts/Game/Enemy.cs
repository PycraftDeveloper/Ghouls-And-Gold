using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float health = 20.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void DealDamage(float Damage)
    {
        health -= Damage;

        if (health <= 0)
        {
            Debug.Log("I'm dead");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("I'm hurt");
        }
    }
}