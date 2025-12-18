using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when a player collides with a token.
    /// </summary>
    public class PlayerTokenCollision : Simulation.Event<PlayerTokenCollision>
    {
        public PlayerController player;
        public TokenInstance token;
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        // Valeur du token, par défaut 10 points
        public int tokenValue = 10;

        public override void Execute()
        {
            // Joue le son de collecte
            AudioSource.PlayClipAtPoint(token.tokenCollectAudio, token.transform.position);

            // Ajoute le score via le modèle
            if (model != null)
            {
                model.AddScore(tokenValue); // <-- ici on utilise le modèle au lieu du ScoreManager
            }

            // Détruire le token
            GameObject.Destroy(token.gameObject);
        }
    }
}
