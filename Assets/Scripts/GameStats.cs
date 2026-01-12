using UnityEngine;
using Platformer.Mechanics; // Needed to find scripts

public static class GameStats
{
    // Live Counters
    public static int Kills = 0;
    public static int Gems = 0;

    // Totals (for the math)
    public static int TotalEnemies = 0;
    public static int TotalGems = 0;

    // Settings
    public const int MaxScore = 1000; // Total score if you do everything 100%

    // Call this when the level starts (we will call it from ScoreUI)
    public static void InitializeLevel()
    {
        Kills = 0;
        Gems = 0;

        // Find how many Tokens are in the scene
        TotalGems = Object.FindObjectsByType<TokenInstance>(FindObjectsSortMode.None).Length;

        // Find how many Enemies are in the scene
        TotalEnemies = Object.FindObjectsByType<EnemyController>(FindObjectsSortMode.None).Length;

        Debug.Log($"Stats Initialized: {TotalGems} Gems, {TotalEnemies} Enemies.");
    }

    public static int CalculateScore()
    {
        // 50% of the score comes from Gems, 50% from Kills
        float maxPointsPerCategory = MaxScore / 2f; 

        float gemPoints = 0;
        float killPoints = 0;

        // Math: (Collected / Total) * 500
        if (TotalGems > 0)
            gemPoints = (float)Gems / TotalGems * maxPointsPerCategory;
        
        if (TotalEnemies > 0)
            killPoints = (float)Kills / TotalEnemies * maxPointsPerCategory;

        return Mathf.RoundToInt(gemPoints + killPoints);
    }
}