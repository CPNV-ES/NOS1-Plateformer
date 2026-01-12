using UnityEngine;

public class StatsExporter : MonoBehaviour
{
    public RedisManager redisManager;

    public string playerName = "Player1";

    float levelStartTime;

    void Start()
    {
        levelStartTime = Time.time;
        GameStats.InitializeLevel();
    }

    public void OnLevelFinished()
    {
        int finalScore = GameStats.CalculateScore();
        float timePlayed = Time.time - levelStartTime;

        LevelStats stats = new LevelStats
        {
            playerName = playerName,
            score = finalScore,
            kills = GameStats.Kills,
            gems = GameStats.Gems,
            timePlayed = timePlayed
        };

        redisManager.SaveStats(stats);

        Debug.Log($"Stats envoyées : {playerName} | Score {finalScore}");
    }
}
