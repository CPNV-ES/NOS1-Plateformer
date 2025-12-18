using UnityEngine;
using TMPro;
using Platformer.Core;
using Platformer.Model;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Update()
    {
        // Récupère le score depuis le modèle
        var model = Simulation.GetModel<PlatformerModel>();
        scoreText.text = "Score: " + model.playerScore;
    }
}
