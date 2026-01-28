using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Loot")]
    [SerializeField] private GameObject GoldItemPrefab;

    [SerializeField] private GameObject HealthItemPrefab;
    [SerializeField] private GameObject ManaItemPrefab;

    [Header("Drop Maths")]
    [SerializeField] private int MinNumberOfItemsToDrop = 1;

    [SerializeField] private int MaxNumberOfItemsToDrop = 3;
    [SerializeField] private float GoldDropChance = 0.75f; // probability
    [SerializeField] private float HealthDropChance = 0.35f;
    [SerializeField] private float ManaDropChance = 0.5f;

    [Header("Chest explode effect")]
    [SerializeField] private float MinItemExplodeForce = 3.0f;

    [SerializeField] private float MaxItemExplodeForce = 6.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private GameObject GetRandomLoot()
    {
        float roll = Random.value;
        float totalChance = GoldDropChance + HealthDropChance + ManaDropChance;

        float goldThreshold = GoldDropChance / totalChance;
        float healthThreshold = goldThreshold + (HealthDropChance / totalChance);

        if (roll < goldThreshold)
            return GoldItemPrefab;
        else if (roll < healthThreshold)
            return HealthItemPrefab;
        else
            return ManaItemPrefab;
    }

    private void ApplyExplosion(GameObject item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();

        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Abs(randomDirection.y);

        float force = Random.Range(MinItemExplodeForce, MaxItemExplodeForce);
        rb.AddForce(randomDirection * force, ForceMode.Impulse);
    }

    public void OnOpen()
    {
        int itemsToDrop = Random.Range(
            MinNumberOfItemsToDrop,
            MaxNumberOfItemsToDrop + 1
        );

        for (int i = 0; i < itemsToDrop; i++)
        {
            GameObject itemPrefab = GetRandomLoot();
            GameObject item = Instantiate(
                itemPrefab,
                transform.position,
                Quaternion.identity
            );

            ApplyExplosion(item);
        }

        Destroy(gameObject);
    }
}