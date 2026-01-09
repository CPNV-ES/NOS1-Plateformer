using UnityEngine;
using System;

public class LeaderboardManager : MonoBehaviour
{
    // We now use the central RedisManager to do the heavy lifting
    public RedisManager redisManager;

    /// <summary>
    /// Fetches the top players from the Unraid API.
    /// This is a bridge method in case other scripts need to trigger a fetch.
    /// </summary>
    public void GetTopPlayers(Action<RedisManager.LeaderboardEntry[]> callback)
    {
        if (redisManager != null)
        {
            redisManager.GetLeaderboard(callback);
        }
        else
        {
            Debug.LogError("LeaderboardManager: RedisManager is not assigned in the inspector!");
            callback?.Invoke(null);
        }
    }

    // This method is kept for compatibility with your older scripts
    // It just redirects the request to the new system.
    public void RefreshLeaderboard()
    {
        if (redisManager != null)
        {
            // We don't do anything with the data here because the UI usually handles the callback
            redisManager.GetLeaderboard(null); 
        }
    }
}