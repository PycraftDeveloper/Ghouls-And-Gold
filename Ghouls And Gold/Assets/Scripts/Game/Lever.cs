using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private GameObject TargetDoor;
    private bool State = false;

    [SerializeField] private Sprite EnabledSprite;
    [SerializeField] private Sprite DisabledSprite;
    private SpriteRenderer LeverRenderer;

    private void Start()
    {
        LeverRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeLeverState()
    {
        if (State == false)
        {
            State = true;

            LeverRenderer.sprite = EnabledSprite;

            TargetDoor.SetActive(true);
        }
        else
        {
            State = false;

            LeverRenderer.sprite = DisabledSprite;

            TargetDoor.SetActive(false);
        }
    }
}