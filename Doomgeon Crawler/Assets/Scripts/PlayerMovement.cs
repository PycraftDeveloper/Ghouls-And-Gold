using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float moveSpeedMult;

    private float moveX;
    private float moveY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.linearVelocity = new Vector3 (moveX * moveSpeedMult, moveY * moveSpeedMult);
    }
}
