using UnityEngine;

public class Resolution : MonoBehaviour
{
    void Start()
    {
        Screen.SetResolution(800, 600, FullScreenMode.MaximizedWindow);
    }

}
