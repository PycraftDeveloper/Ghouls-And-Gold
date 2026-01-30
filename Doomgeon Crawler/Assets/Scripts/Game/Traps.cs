using UnityEngine;

public class Traps : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject tripWire;
    [SerializeField] private GameObject[] projectileSpawnPoints;
    [SerializeField] private GameObject projectile;

    [Header("Trap Settings")]
    [SerializeField] private Vector3 projectileDirection;
    [SerializeField] private float velocity;

    public void TrapActivated()
    {
        foreach (GameObject spawnPoint in projectileSpawnPoints)
        {
            Instantiate(projectile);
            projectile.GetComponent<Rigidbody>().AddForce(velocity * projectileDirection);
        }
    }
}
