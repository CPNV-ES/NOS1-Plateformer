using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LeaderboardUI : MonoBehaviour
{
    public string baseUrl = "http://localhost:7379";
    public int topCount = 10;
    public TMP_Text leaderboardText;

    void Start()
    {
        StartCoroutine(GetTopPlayersRoutine(topCount));
    }

    private IEnumerator GetTopPlayersRoutine(int count)
    {
        string url = $"{baseUrl}/ZREVRANGE/leaderboard/0/{count - 1}/WITHSCORES";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erreur leaderboard : " + request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                var leaderboard = ParseLeaderboard(json);
                DisplayLeaderboard(leaderboard);
            }
        }
    }

    private List<KeyValuePair<string, int>> ParseLeaderboard(string json)
    {
        List<KeyValuePair<string, int>> leaderboard = new List<KeyValuePair<string, int>>();

        // Extraire la valeur entre ["..."] de {"ZREVRANGE":[...]}
        Match match = Regex.Match(json, @"\[(.*)\]");
        if (!match.Success) return leaderboard;

        string content = match.Groups[1].Value;
        string[] entries = content.Split(new string[] { "\",\"" }, System.StringSplitOptions.None);

        for (int i = 0; i < entries.Length; i += 2)
        {
            string playerName = entries[i].Replace("\"", "");
            if (i + 1 >= entries.Length) break;

            if (int.TryParse(entries[i + 1].Replace("\"", ""), out int score))
            {
                leaderboard.Add(new KeyValuePair<string, int>(playerName, score));
            }
        }

        return leaderboard;
    }

    private void DisplayLeaderboard(List<KeyValuePair<string, int>> leaderboard)
    {
        leaderboardText.text = "";
        int rank = 1;

        foreach (var entry in leaderboard)
        {
            leaderboardText.text += $"{rank}. {entry.Key} : {entry.Value}\n";
            rank++;
        }
    }
}
