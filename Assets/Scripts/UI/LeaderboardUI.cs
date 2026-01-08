using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Settings")]
    public string baseUrl = "http://localhost:7379";
    public int topCount = 10;
    public TMP_Text leaderboardText;

    // Helper class to hold our decoded data
    private class LeaderboardEntry
    {
        public string playerName;
        public int score;
        public float time;
    }

    void OnEnable()
    {
        // Refresh leaderboard every time the UI is opened
        RefreshLeaderboard();
    }

    public void RefreshLeaderboard()
    {
        StopAllCoroutines();
        StartCoroutine(GetTopPlayersRoutine(topCount));
    }

    private IEnumerator GetTopPlayersRoutine(int count)
    {
        // ZREVRANGE gets the highest scores first
        string url = $"{baseUrl}/ZREVRANGE/leaderboard/0/{count - 1}/WITHSCORES";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Leaderboard Error: " + request.error);
                if (leaderboardText != null) leaderboardText.text = "Error loading leaderboard.";
            }
            else
            {
                string json = request.downloadHandler.text;
                List<LeaderboardEntry> leaderboard = ParseLeaderboard(json);
                DisplayLeaderboard(leaderboard);
            }
        }
    }

    private List<LeaderboardEntry> ParseLeaderboard(string json)
    {
        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        // Extract everything inside the square brackets [ ... ]
        Match match = Regex.Match(json, @"\[(.*)\]");
        if (!match.Success) return entries;

        string content = match.Groups[1].Value;
        if (string.IsNullOrEmpty(content)) return entries;

        // Split by "," (Webdis format)
        string[] rawData = content.Split(new string[] { "\",\"" }, System.StringSplitOptions.None);

        for (int i = 0; i < rawData.Length; i += 2)
        {
            // Clean up name and score strings
            string name = rawData[i].Replace("\"", "");
            if (i + 1 >= rawData.Length) break;
            
            string scoreRaw = rawData[i + 1].Replace("\"", "");

            // Use InvariantCulture to handle dots vs commas in different regions
            if (double.TryParse(scoreRaw, NumberStyles.Any, CultureInfo.InvariantCulture, out double compositeScore))
            {
                // DECODE MATH:
                // compositeScore = Score + (1 / (Time + 1))
                
                int score = (int)System.Math.Floor(compositeScore);
                double decimalPart = compositeScore - score;

                float time = 0f;
                if (decimalPart > 0)
                {
                    // Reverse the math: time = (1 / decimal) - 1
                    time = (float)((1.0 / decimalPart) - 1.0);
                }

                entries.Add(new LeaderboardEntry 
                { 
                    playerName = name, 
                    score = score, 
                    time = time 
                });
            }
        }

        return entries;
    }

    private void DisplayLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        string header = "<u><b>RANK<pos=15%>NAME<pos=55%>SCORE<pos=80%>TIME</b></u>\n";
        
        header += "\n"; 

        string rows = "";
        int rank = 1;
        foreach (var entry in leaderboard)
        {
            rows += $"{rank}.<pos=15%>{entry.playerName}<pos=55%>{entry.score}<pos=80%>{entry.time:F1}s\n";
            rank++;
        }

        leaderboardText.text = header + rows;
    }
}