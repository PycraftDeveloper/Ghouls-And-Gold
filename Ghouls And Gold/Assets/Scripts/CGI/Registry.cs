using UnityEngine;

// This class stores a shared collection of variables under a global namespace "Registry".
public static class Registry
{
    public static Player PlayerObject;
    public static CoreGameInfrastructure CoreGameInfrastructureObject;
    public static bool IsPaused = false;

    public static float SFX_Volume = 1.0f;
    public static float Music_Volume = 1.0f;
    public static float Master_Volume = 1.0f;
}