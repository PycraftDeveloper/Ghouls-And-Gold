using UnityEngine;

public class Tripwire : MonoBehaviour
{
    [SerializeField] private Traps trapToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            trapToActivate.TrapActivated();
        }
    }
}
