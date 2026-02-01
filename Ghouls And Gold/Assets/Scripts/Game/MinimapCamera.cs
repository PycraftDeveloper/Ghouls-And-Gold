using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private void Update()
    {
        transform.position = Camera.main.transform.position;
    }
}
