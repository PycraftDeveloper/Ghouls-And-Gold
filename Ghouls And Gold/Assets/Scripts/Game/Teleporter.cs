using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform teleportPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetPositionAndRotation(teleportPoint.position, teleportPoint.rotation);
        }
    }
}
