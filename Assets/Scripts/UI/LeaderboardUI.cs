using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public RedisManager redisManager;
    public TMP_Text leaderboardText;

    void OnEnable() {
        Refresh();
    }

    public void Refresh() {
        if (leaderboardText != null) leaderboardText.text = "Loading...";
        
        redisManager.GetLeaderboard((entries) => {
            if (entries == null || entries.Length == 0) {
                leaderboardText.text = "No scores found.";
                return;
            }

            string text = "<u><b>RANK<pos=15%>NAME<pos=55%>SCORE<pos=80%>TIME</b></u>\n\n";

            for (int i = 0; i < entries.Length; i++) {
                string playerName = entries[i].name;
                int playerScore = entries[i].score;
                float playerTime = entries[i].time;

                text += $"{i + 1}.<pos=15%>{playerName}<pos=55%>{playerScore}<pos=80%>{playerTime:F1}s\n";
            }

            leaderboardText.text = text;
        });
    }
}