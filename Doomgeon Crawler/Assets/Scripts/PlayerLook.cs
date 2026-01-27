using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class PlayerLook : MonoBehaviour
{
    private Gamepad controller = null;
    private Transform m_transform;


    void Start()
    {
        this.controller = DS4.getController();
        m_transform = this.transform;
    }


    void Update()
    {
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
            m_transform.rotation *= DS4.getRotation(4000 * Time.deltaTime);
        }
    }
}
