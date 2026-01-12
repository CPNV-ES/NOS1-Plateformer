using Platformer.Mechanics;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

public class VictoryZone : MonoBehaviour
{
    public RedisManager redisManager;
    public VictoryUI victoryUI;
    private GameTimer timer;
    private bool _isProcessing = false; // Spam safeguard

    void Start()
    {
        timer = Object.FindAnyObjectByType<GameTimer>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (_isProcessing) return; // Prevent multiple triggers if physics glitches

        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _isProcessing = true;

            // 1. Calculate Score
            int finalScore = GameStats.CalculateScore();
            float finalTime = (timer != null) ? timer.FinalTime : 0f;

            // 2. Prepare UI
            if (victoryUI != null) victoryUI.ShowVictory(finalScore);

            // 3. Create Stats Object (deviceId is filled automatically by RedisManager)
            LevelStats stats = new LevelStats
            {
                playerName = PlayerPrefs.GetString("PlayerName", "Unknown"),
                score = finalScore,
                kills = GameStats.Kills,
                gems = GameStats.Gems,
                timePlayed = finalTime
            };

            // 4. Send to API
            if (redisManager != null)
            {
                redisManager.SaveStats(stats, (response) => 
                {
                    if (victoryUI != null)
                    {
                        victoryUI.UpdateHighScoreMessage(response);
                    }
                });
            }

            // 5. Trigger Game Victory Event
            var ev = Schedule<PlayerEnteredVictoryZone>();
            ev.victoryZone = this;
        }
    }
}