using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        // When the UI loads, we count the enemies/gems once.
        GameStats.InitializeLevel();
    }

    void Update()
    {
        // Retrieve the calculated score based on Kills/Gems
        int currentScore = GameStats.CalculateScore();
        scoreText.text = "Score: " + currentScore;
    }
}