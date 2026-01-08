using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System; // Needed for Action<bool>

public class RedisManager : MonoBehaviour
{
    // Webdis URL
    public string baseUrl = "http://localhost:7379";

    // =========================
    // SAVE STATS (With Check)
    // =========================
    // We added 'Action<bool> onResult' to let the caller know if it was a high score
    public void SaveStats(LevelStats stats, Action<bool> onResult = null)
    {
        StartCoroutine(SaveStatsRoutine(stats, onResult));
    }

    private IEnumerator SaveStatsRoutine(LevelStats newStats, Action<bool> onResult)
    {
        string key = "stats:" + newStats.playerName;
        string getUrl = $"{baseUrl}/GET/{key}";

        bool isNewHighScore = false;

        // STEP 1: Check existing data in DB
        using (UnityWebRequest getRequest = UnityWebRequest.Get(getUrl))
        {
            yield return getRequest.SendWebRequest();

            if (getRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Database Error (GET): " + getRequest.error);
                // If DB fails, we assume it's true just to be safe, or false to prevent overwrites. 
                // Let's stop to be safe.
                onResult?.Invoke(false);
                yield break; 
            }

            string jsonResponse = getRequest.downloadHandler.text;

            // If "GET": null, player doesn't exist yet -> Save it.
            if (jsonResponse.Contains(":null") || jsonResponse.Contains(": null"))
            {
                isNewHighScore = true;
            }
            else
            {
                // Parse the Webdis wrapper
                WebdisResponse wrapper = JsonUtility.FromJson<WebdisResponse>(jsonResponse);
                
                if (!string.IsNullOrEmpty(wrapper.GET))
                {
                    LevelStats oldStats = JsonUtility.FromJson<LevelStats>(wrapper.GET);

                    // LOGIC: New Score > Old Score OR (Same Score AND New Time < Old Time)
                    if (newStats.score > oldStats.score)
                    {
                        isNewHighScore = true;
                    }
                    else if (newStats.score == oldStats.score && newStats.timePlayed < oldStats.timePlayed)
                    {
                        isNewHighScore = true;
                    }
                }
                else
                {
                    // If wrapper was empty for some reason, treat as new
                    isNewHighScore = true;
                }
            }
        }

        // STEP 2: Save only if it is a new high score
        if (isNewHighScore)
        {
            string json = JsonUtility.ToJson(newStats);
            string setUrl = $"{baseUrl}/SET/{key}/{UnityWebRequest.EscapeURL(json)}";

            using (UnityWebRequest setRequest = UnityWebRequest.Get(setUrl))
            {
                yield return setRequest.SendWebRequest();
                
                if (setRequest.result == UnityWebRequest.Result.Success)
                    Debug.Log($"Stats updated for {newStats.playerName}!");
                else
                    Debug.LogError("Error saving stats: " + setRequest.error);
            }
        }
        else
        {
            Debug.Log($"Score ({newStats.score}) was not better than existing record. Save skipped.");
        }

        // STEP 3: Tell the VictoryZone (and UI) what happened
        onResult?.Invoke(isNewHighScore);
    }

    // =========================
    // LEADERBOARD
    // =========================
    public void AddScoreToLeaderboard(string playerName, int score, float time)
    {
        StartCoroutine(AddScoreRoutine(playerName, score, time));
    }

    private IEnumerator AddScoreRoutine(string playerName, int score, float time)
    {
        // Composite Score Math: Score + (1 / (Time + 1))
        double compositeScore = (double)score + (1.0 / (double)(time + 1.0));
        
        // F6 for precision, InvariantCulture for dots instead of commas
        string scoreStr = compositeScore.ToString("F6", System.Globalization.CultureInfo.InvariantCulture);
        
        string url = $"{baseUrl}/ZADD/leaderboard/{scoreStr}/{UnityWebRequest.EscapeURL(playerName)}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                Debug.LogError("Leaderboard Error: " + request.error);
        }
    }
}

// Helper class to read Webdis JSON
[Serializable]
public class WebdisResponse
{
    public string GET;
}