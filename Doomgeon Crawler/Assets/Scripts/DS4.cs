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
        string layout = File.ReadAllText(layoutFile == null ? "Assets/Scripts/CustomDualShockLayout.json" : layoutFile);

        // Overwrite the default layout
        InputSystem.RegisterLayoutOverride(layout, "DualSenseGamepadHID2");

        var ds4 = Gamepad.current;
        DS4.controller = ds4;
        bindControls(DS4.controller);
        return DS4.controller;
    }

    private static void bindControls(Gamepad ds4)
    {
        gyroZ = ds4.GetChildControl<ButtonControl>("accl Y 21"); //!
        gyroY = ds4.GetChildControl<ButtonControl>("accl X 19"); //!
        gyroX = ds4.GetChildControl<ButtonControl>("gyro Z 17"); //!

        gyroPosZ = ds4.GetChildControl<ButtonControl>("accl X 19");
    }

    public static Quaternion getRotation(float scale = 1)
    {
        float x = processRawData(gyroX.ReadValue()) * scale;
        float y = processRawData(gyroY.ReadValue()) * scale;
        float z = -processRawData(gyroZ.ReadValue()) * scale;
        return Quaternion.Euler(x, y, z);
    }

    private static float processRawData(float data)
    {
        return data > 0.5 ? 1 - data : -data;
    }

    public static Vector3 getPosition(float scale = 1)
    {
        float z = processRawData(gyroPosZ.ReadValue()) * scale;
        return new Vector3(0f, 0f, z);
    }
}
