using TMPro;
using UnityEngine;

/// <summary>
/// Représente un minuteur affichant le temps restant au format MM:SS.
/// </summary>
public class MinuteurAffichage : MonoBehaviour
{
    [SerializeField] private float dureePartie = 120f;          // Temps de jeu en secondes
    [SerializeField] private TextMeshProUGUI minuteurText;      // Texte TMP pour afficher le temps

    private float tempsRestant;

    private void Start()
    {
        tempsRestant = dureePartie;
        MettreAJourTexte();
    }

    private void Update()
    {
        // Si la partie est déjà terminée, on arrête le timer
        if (GestionFinPartie.Instance != null && GestionFinPartie.Instance.PartieTerminee)
            return;

        tempsRestant -= Time.deltaTime;

        if (tempsRestant <= 0f)
        {
            tempsRestant = 0f;

            // Déclare la défaite une seule fois
            if (GestionFinPartie.Instance != null)
            {
                GestionFinPartie.Instance.DeclarerDefaite();
            }
        }

        MettreAJourTexte();
    }

    private void MettreAJourTexte()
    {
        int minutes = Mathf.FloorToInt(tempsRestant / 60f);
        int secondes = Mathf.FloorToInt(tempsRestant % 60f);
        minuteurText.text = string.Format("{0:00}:{1:00}", minutes, secondes);
    }
}
