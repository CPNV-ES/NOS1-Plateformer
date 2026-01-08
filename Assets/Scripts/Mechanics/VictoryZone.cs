using Platformer.Gameplay;
using Platformer.Mechanics; 
using UnityEngine;
using static Platformer.Core.Simulation;

public class VictoryZone : MonoBehaviour
{
    [Header("Database Connection")]
    public RedisManager redisManager;

    [Header("UI")]
    public VictoryUI victoryUI;

    [Header("Player Settings")]
    public string playerPrefsKey = "PlayerName";

    // Private because we find it automatically
    private GameTimer timer;

    void Start()
    {
        // Auto-find timer to avoid manual setup errors
        timer = Object.FindAnyObjectByType<GameTimer>();
        if (timer == null) Debug.LogError("VictoryZone: No GameTimer found in the scene!");
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // 1. Calculate Data
            int finalScore = GameStats.CalculateScore();
            float finalTime = (timer != null) ? timer.FinalTime : 9999f; 

            // 2. Show UI immediately (Loading state)
            if (victoryUI != null)
            {
                victoryUI.timer = timer; 
                victoryUI.ShowVictory(finalScore);
            }

            // 3. Create Stats Object
            LevelStats stats = new LevelStats
            {
                playerName = PlayerPrefs.GetString(playerPrefsKey, "Unknown"),
                score = finalScore,
                kills = GameStats.Kills,
                gems = GameStats.Gems,
                timePlayed = finalTime
            };

            // 4. Send to Redis with a Callback
            if (redisManager != null)
            {
                // This block runs AFTER Redis checks the database
                redisManager.SaveStats(stats, (isNewHighScore) => 
                {
                    // A. Update UI Text
                    if (victoryUI != null)
                    {
                        victoryUI.UpdateHighScoreMessage(isNewHighScore);
                    }

                    // B. If it IS a new record, update the Global Leaderboard too
                    if (isNewHighScore)
                    {
                        redisManager.AddScoreToLeaderboard(stats.playerName, stats.score, stats.timePlayed);
                    }
                });
            }

            // 5. Game Event (Original Logic)
            var ev = Schedule<PlayerEnteredVictoryZone>();
            ev.victoryZone = this;
        }
    }
}