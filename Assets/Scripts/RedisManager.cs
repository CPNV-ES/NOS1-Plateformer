using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RedisManager : MonoBehaviour
{
    // The address of your Webdis/Redis server
    private string baseUrl = "http://localhost:7379"; 

    public void SaveStats(LevelStats stats)
    {
        StartCoroutine(SaveStatsRoutine(stats));
    }

    private IEnumerator SaveStatsRoutine(LevelStats stats)
    {
        // 1. Convert the C# object to a JSON string
        // Result looks like: {"playerName":"Player1","kills":5...}
        string json = JsonUtility.ToJson(stats);

        // 2. Prepare the Key and Value
        // Key: We use "stats:" + name (e.g., stats:Player1)
        string key = "stats:" + stats.playerName;

        // 3. Construct the URL for Webdis
        // Format: http://localhost:7379/SET/key/value
        // IMPORTANT: We must "Escape" the JSON string so symbols like "{" don't break the URL
        string url = $"{baseUrl}/SET/{key}/{UnityWebRequest.EscapeURL(json)}";

        // 4. Send the Request
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Wait for it to finish
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Redis Error: " + webRequest.error);
            }
            else
            {
                Debug.Log("Success! Stats saved to Redis.");
                Debug.Log("Server Response: " + webRequest.downloadHandler.text);
            }
        }
    }
}