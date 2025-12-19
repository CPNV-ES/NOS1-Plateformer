using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



public class MenuManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject MainMenuCanvas;
    public GameObject AskNameCanvas;

    [Header("Input")]
    public TMP_InputField nameInput;

    public void OnStartButton()
    {
        if(MainMenuCanvas != null) MainMenuCanvas.SetActive(false);
        if(AskNameCanvas != null) AskNameCanvas.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    public void OnNameSubmit()
    {
        if(nameInput == null) { Debug.LogError("nameInput non assigné !"); return; }
        if(AskNameCanvas == null) { Debug.LogError("AskNameCanvas non assigné !"); return; }

        Debug.Log("InputField text = '" + nameInput.text + "'");

        string playerName = nameInput.text.Trim();

        if(string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Merci d’entrer un nom !");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        AskNameCanvas.SetActive(false);

        SceneManager.LoadScene("SampleScene");
    }
    public void OnQuitButton()
    {
        Debug.Log("Quitter le jeu");
        Application.Quit();
    }
}
