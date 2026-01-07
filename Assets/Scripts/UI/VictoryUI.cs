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
    public GameTimer timer; // référence au GameTimer

    void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowVictory(int score)
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (messageText != null)
            messageText.text = "Ton score final est :";

        if (finalScoreText != null)
            finalScoreText.text = score.ToString();

        // Récupère le temps directement depuis le GameTimer
        if (finalTimeText != null && timer != null)
            finalTimeText.text = $"Temps : {timer.FinalTime:0.0}s";


    }

    public void HideVictory()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
