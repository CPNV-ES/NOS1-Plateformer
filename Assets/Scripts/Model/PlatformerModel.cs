using Platformer.Mechanics;
using UnityEngine;

namespace Platformer.Model
{
    [System.Serializable]
    public class PlatformerModel
    {
        public Unity.Cinemachine.CinemachineCamera virtualCamera;
        public PlayerController player;
        public Transform spawnPoint;
        public float jumpModifier = 1.5f;
        public float jumpDeceleration = 0.5f;

        // -------------------------
        // Nouveau champ score
        // -------------------------
        public int playerScore = 0;

        /// <summary>
        /// Ajouter au score
        /// </summary>
        public void AddScore(int amount)
        {
            playerScore += amount;
            Debug.Log("Score: " + playerScore);
        }

        /// <summary>
        /// Réinitialiser le score
        /// </summary>
        public void ResetScore()
        {
            playerScore = 0;
        }
    }
}
