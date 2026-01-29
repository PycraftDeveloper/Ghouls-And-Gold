using System.Collections;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private GameObject TargetDoor;
    [SerializeField] private float LeverSwapTime = 0.75f;
    private Coroutine LeverStateCoroutine;
    private bool State = false;

    [SerializeField] private Quaternion EnabledOffset;
    [SerializeField] private Quaternion DisabledOffset;

    private IEnumerator ActivateLever()
    {
        float Duration = 0;
        Quaternion StartAngle = transform.localRotation;
        while (Duration < LeverSwapTime)
        {
            Duration += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(StartAngle, EnabledOffset, Duration / LeverSwapTime);
            yield return null;
        }

        TargetDoor.SetActive(false);
    }

    private IEnumerator DeactivateLever()
    {
        float Duration = 0;
        Quaternion StartAngle = transform.localRotation;
        while (Duration < LeverSwapTime)
        {
            Duration += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(StartAngle, DisabledOffset, Duration / LeverSwapTime);
            yield return null;
        }

        TargetDoor.SetActive(true);
    }

    public void ChangeLeverState()
    {
        if (LeverStateCoroutine != null)
        {
            StopCoroutine(LeverStateCoroutine);
        }

        if (State == false)
        {
            State = true;

            LeverStateCoroutine = StartCoroutine(ActivateLever());
        }
        else
        {
            State = false;
            LeverStateCoroutine = StartCoroutine(DeactivateLever());
        }
    }
}