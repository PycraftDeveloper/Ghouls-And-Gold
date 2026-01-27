using System;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class PlayerLook : MonoBehaviour
{
    private Gamepad controller = null;
    private Transform m_transform;

    public float GyroRotationSpeed = 4000.0f;
    public float AxisRotationSpeed = 100.0f;

    private PlayerInput inputActions;
    private Quaternion AxisLookRotation;

    private void Start()
    {
        this.controller = DS4.getController();
        m_transform = this.transform;
    }

    private void Awake()
    {
        inputActions = new PlayerInput();

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 Axis = context.ReadValue<Vector2>();
        AxisLookRotation = Quaternion.Euler(-Axis.y * Time.deltaTime * AxisRotationSpeed, Axis.x * Time.deltaTime * AxisRotationSpeed, 0);
    }

    private void Update()
    {
        Quaternion CombinedRotation = m_transform.rotation;

        if (controller == null)
        {
            try
            {
                controller = DS4.getController();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        else
        {
            if (controller.rightShoulder.isPressed)
            {
                m_transform.rotation = Quaternion.identity;
            }
            CombinedRotation *= DS4.getRotation(GyroRotationSpeed * Time.deltaTime);
        }

        CombinedRotation *= AxisLookRotation;
        // prevent unintended camera movement here
        Vector3 EulerRotation = CombinedRotation.eulerAngles;

        float pitch = EulerRotation.x;
        if (pitch > 180f)
            pitch -= 360f;

        // Clamp to prevent looking up/down issue
        pitch = Mathf.Clamp(pitch, -70f, 70f);

        m_transform.rotation = Quaternion.Euler(pitch, EulerRotation.y, 0);
    }
}