using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

namespace Platformer.UI
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        public GameObject[] panels;

        public void SetActivePanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                var active = i == index;
                var g = panels[i];
                if (g.activeSelf != active) g.SetActive(active);
            }
        }

        // Add this new method to restart the game
        public void RestartGame()
        {
            // This line gets the name of the currently active scene and reloads it.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }


        void OnEnable()
        {
            SetActivePanel(0);
        }
    }
}