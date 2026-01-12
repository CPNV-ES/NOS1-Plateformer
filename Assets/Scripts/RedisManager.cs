using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class RedisManager : MonoBehaviour
{
    [Serializable]
    private class Config { 
        public string baseUrl;
        public string apiKey; 
    }

    private Config _config;

    void Awake()
    {
        // Load config from Resources/config.json
        TextAsset configAsset = Resources.Load<TextAsset>("config");
        if (configAsset != null)
        {
            _config = JsonUtility.FromJson<Config>(configAsset.text);
            
            // Safety check: Remove trailing slash if it exists
            if (_config.baseUrl.EndsWith("/")) 
                _config.baseUrl = _config.baseUrl.TrimEnd('/');
        }
        else
        {
            Debug.LogError("RedisManager: Assets/Resources/config.json not found!");
        }
    }

    // ==========================================
    // SAVE STATS
    // ==========================================
    // 2. Update the SaveStats method signature
    public void SaveStats(LevelStats stats, Action<SaveResponse> onResult = null)
    {
        if (_config == null) return;
        StartCoroutine(SaveStatsRoutine(stats, onResult));
    }

    // 3. Update the Routine to parse the JSON
    private IEnumerator SaveStatsRoutine(LevelStats stats, Action<SaveResponse> onResult)
    {
        stats.playerName = stats.playerName.Trim();
        stats.deviceId = SystemInfo.deviceUniqueIdentifier;
        string jsonData = JsonUtility.ToJson(stats);
        string url = $"{_config.baseUrl}/save";

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-KEY", _config.apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the response from Python
                string json = request.downloadHandler.text;
                SaveResponse response = JsonUtility.FromJson<SaveResponse>(json);
                
                onResult?.Invoke(response);
            }
            else
            {
                Debug.LogError($"Save Failed: {request.downloadHandler.text}");
                onResult?.Invoke(null);
            }
        }
    }

    // ==========================================
    // GET LEADERBOARD
    // ==========================================
    public void GetLeaderboard(Action<LeaderboardEntry[]> onCallback)
    {
        if (_config == null) return;
        StartCoroutine(GetLeaderboardRoutine(onCallback));
    }

    private IEnumerator GetLeaderboardRoutine(Action<LeaderboardEntry[]> onCallback)
    {
        // Use clean baseUrl + endpoint
        string url = $"{_config.baseUrl}/leaderboard";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the JSON Array using our Helper
                LeaderboardEntry[] entries = JsonHelper.FromJson<LeaderboardEntry>(request.downloadHandler.text);
                onCallback?.Invoke(entries);
            }
            else
            {
                Debug.LogError($"Leaderboard fetch failed: {request.error}");
                onCallback?.Invoke(null);
            }
        }
    }
    
    // --- HELPER CLASSES ---
    [Serializable]
    public class LeaderboardEntry {
        public string name;
        public int score;
        public float time;
    }

    // Helper to handle JSON arrays (Unity's JsonUtility cannot parse top-level arrays directly)
    private static class JsonHelper {
        public static T[] FromJson<T>(string json) {
            string newJson = "{ \"items\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.items;
        }
        [Serializable] private class Wrapper<T> { public T[] items; }
    }
}

[Serializable]
public class SaveResponse
{
    public string status;
    public int currentScore;
    public int previousBest;
}