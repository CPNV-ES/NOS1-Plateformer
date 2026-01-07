using UnityEngine;
using TMPro;

public class HudUi : MonoBehaviour
{
    [Header("HUD Elements")]
    public TMP_Text scoreText;
    public TMP_Text timerText;  

    [Header("References")]
    public GameTimer timer;     

    void Start()
    {
        GameStats.InitializeLevel();
    }

    void Update()
    {
        // Met à jour le score
        if (scoreText != null)
        {
            int currentScore = GameStats.CalculateScore();
            scoreText.text = "Score : " + currentScore;
        }

        // Met à jour le timer
        if (timer != null && timerText != null)
        {
            timerText.text = "Temps : " + timer.FinalTime.ToString("0.0") + "s";
        }
    }
}
