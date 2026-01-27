using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DS4
{
    // Gyroscope
    public static ButtonControl gyroX = null;

    public static ButtonControl gyroY = null;
    public static ButtonControl gyroZ = null;
    public static ButtonControl gyroPosZ = null;

    public static Gamepad controller = null;

    public static Gamepad getController(string layoutFile = null)
    {
        // Read layout from JSON file
        string layout = File.ReadAllText(layoutFile == null ? "Assets/Scripts/Utility/CustomDualShockLayout.json" : layoutFile);

        // Overwrite the default layout
        InputSystem.RegisterLayoutOverride(layout, "DualSenseGamepadHID2");

        var ds4 = Gamepad.current;

        if (ds4 == null)
        {
            Debug.LogError("Please connect a PS5 controller.");
            return null;
        }

        DS4.controller = ds4;
        bindControls(DS4.controller);
        return DS4.controller;
    }

    private static void bindControls(Gamepad ds4)
    {
        try
        {
            gyroZ = ds4.GetChildControl<ButtonControl>("accl Y 21"); //!
        }
        catch
        {
            gyroZ = null;
            Debug.LogWarning("This controller does not have gyro..");
        }

        try
        {
            gyroY = ds4.GetChildControl<ButtonControl>("accl X 19"); //!
        }
        catch
        {
            gyroY = null;
            Debug.LogWarning("This controller does not have gyro.");
        }

        try
        {
            gyroX = ds4.GetChildControl<ButtonControl>("gyro Z 17"); //!
        }
        catch
        {
            gyroX = null;
            Debug.LogWarning("This controller does not have gyro.");
        }

        try
        {
            gyroPosZ = ds4.GetChildControl<ButtonControl>("accl X 19");
        }
        catch
        {
            gyroPosZ = null;
            Debug.LogWarning("This controller does not have gyro.");
        }
    }

    public static Quaternion getRotation(float scale = 1)
    {
        float x = 0;
        float y = 0;
        float z = 0;

        if (gyroX != null)
            x = processRawData(gyroX.ReadValue()) * scale;

        if (gyroY != null)
            y = processRawData(gyroY.ReadValue()) * scale;

        if (gyroZ != null)
            z = -processRawData(gyroZ.ReadValue()) * scale;

        return Quaternion.Euler(x, y, z);
    }

    private static float processRawData(float data)
    {
        return data > 0.5 ? 1 - data : -data;
    }

    public static Vector3 getPosition(float scale = 1)
    {
        float z = 0;
        if (gyroPosZ != null)
            z = processRawData(gyroPosZ.ReadValue()) * scale;
        else
            z = 0;

        return new Vector3(0f, 0f, z);
    }
}