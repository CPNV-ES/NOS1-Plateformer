using UnityEngine;

public static class GameStats
{
    // Static variables live across the whole game session
    public static int Kills = 0;
    public static int Gems = 0;

    // Call this at the start of the level (e.g., in PlayerController Awake)
    public static void Reset()
    {
        Kills = 0;
        Gems = 0;
    }
}