using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject victoryPanel;

    [Header("Texts")]
    public TMP_Text messageText;
    public TMP_Text finalScoreText;
    public TMP_Text finalTimeText; 

    [Header("Timer Reference")]
    public GameTimer timer; 

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowVictory(int score)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // Initial state while waiting for database
        if (messageText != null)
        {
            messageText.text = "Vérification...";
            messageText.color = Color.white;
        }

        if (finalScoreText != null)
            finalScoreText.text = score.ToString();

        if (finalTimeText != null && timer != null)
            finalTimeText.text = $"Temps : {timer.FinalTime:0.0}s";

        // Pause game
        Time.timeScale = 0f;
    }

    public void UpdateHighScoreMessage(SaveResponse response)
    {
        if (messageText == null) return;

        if (response == null)
        {
            messageText.text = "Offline Mode";
            return;
        }

        if (response.status == "new_record")
        {
            // CASE 1: New Personal Best
            messageText.text = "NOUVEAU RECORD PERSO !";
            messageText.color = Color.green;

            if (finalScoreText != null)
            {
                int diff = response.currentScore - response.previousBest;
                finalScoreText.text = $"{response.currentScore} <size=60%><color=green>(+{diff})</color></size>";
            }
        }
        else
        {
            // CASE 2: Not good enough
            messageText.text = "Record Perso Non Battu";
            messageText.color = Color.yellow;

            if (finalScoreText != null)
            {
                int diff = response.previousBest - response.currentScore;
                finalScoreText.text = $"{response.currentScore} <size=60%><color=red>(-{diff})</color></size>";
            }
        }
    }

    public void HideVictory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}