using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText; // texte affiché dans le HUD

    [Header("Timer Settings")]
    public float currentTime = 0f;
    public bool countDown = false;

    [Header("Limit Settings")]
    public bool hasLimit = false;
    public float timerLimit = 0f;

    void Update()
    {
        if (countDown)
            currentTime -= Time.deltaTime;
        else
            currentTime += Time.deltaTime;

        // Vérification limite
        if (hasLimit && ((countDown && currentTime <= timerLimit) || (!countDown && currentTime >= timerLimit)))
        {
            currentTime = timerLimit;
            if (timerText != null)
                timerText.color = Color.red;
            enabled = false;
        }

        // Mise à jour du texte
        if (timerText != null)
            timerText.text = "Temps : " + currentTime.ToString("0.0") + "s";
    }

    public float FinalTime => currentTime; // pour VictoryUI et VictoryZone
}
