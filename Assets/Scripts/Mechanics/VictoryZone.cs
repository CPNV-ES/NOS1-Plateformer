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

    [Header("Timer Reference")]
    public GameTimer timer;

    [Header("Player Settings")]
    public string playerPrefsKey = "PlayerName";

    void OnTriggerEnter2D(Collider2D collider)
    {
        var player = collider.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            // Calcul du score et du temps
            int finalScore = GameStats.CalculateScore();

            // Stats pour Redis
            LevelStats stats = new LevelStats
            {
                playerName = PlayerPrefs.GetString(playerPrefsKey, "Unknown"),
                score = finalScore,
                kills = GameStats.Kills,
                gems = GameStats.Gems,
                timePlayed = timer != null ? timer.FinalTime : 0f
            };

            if (redisManager != null)
                redisManager.SaveStats(stats);

            // Affichage de la fenêtre de victoire
            if (victoryUI != null)
            {
                victoryUI.timer = timer; // on relie le timer au VictoryUI
                victoryUI.ShowVictory(finalScore);
            }

            // Logique originale
            var ev = Schedule<PlayerEnteredVictoryZone>();
            ev.victoryZone = this;
        }
    }
}
