using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// Marks a trigger as a VictoryZone, usually used to end the current game level.
    /// </summary>
    public class VictoryZone : MonoBehaviour
    {
        [Header("Database Connection")]
        [Tooltip("Drag your RedisManager GameObject here")]
        public RedisManager redisManager;

        void OnTriggerEnter2D(Collider2D collider)
        {
            var p = collider.gameObject.GetComponent<PlayerController>();
            if (p != null)
            {
                // --- REDIS SAVING LOGIC START ---
                if (redisManager != null)
                {
                    // 1. Collect your stats
                    // Note: You need to replace the '0's below with the actual variables 
                    // from your GameManager, PlayerController, or TokenController.
                    LevelStats stats = new LevelStats();
                    
                    stats.playerName = PlayerPrefs.GetString("PlayerName", "Unknown");
                    stats.timeElapsed = Time.timeSinceLevelLoad; // Current level time
                    stats.percentage = 100f; // They finished, so 100%
                    stats.gems = GameStats.Gems;
                    stats.kills = GameStats.Kills;

                    // 2. Send to Redis
                    redisManager.SaveStats(stats);
                }
                else
                {
                    Debug.LogWarning("RedisManager is not assigned in the VictoryZone Inspector!");
                }
                // --- REDIS SAVING LOGIC END ---

                // Execute the original game winning logic
                var ev = Schedule<PlayerEnteredVictoryZone>();
                ev.victoryZone = this;
            }
        }
    }
}