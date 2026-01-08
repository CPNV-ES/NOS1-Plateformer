using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject victoryPanel;

    [Header("Texts")]
    public TMP_Text messageText; // The text that says "Score Final" or "New Record"
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

    // Called by VictoryZone after Redis responds
    public void UpdateHighScoreMessage(bool isNewRecord)
    {
        if (messageText == null) return;

        if (isNewRecord)
        {
            messageText.text = "NOUVEAU RECORD !";
            messageText.color = Color.green; 
        }
        else
        {
            messageText.text = "Score non battu";
            messageText.color = Color.yellow; 
        }
    }

    public void HideVictory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}