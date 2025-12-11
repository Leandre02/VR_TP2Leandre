using UnityEngine;

/// <summary>
/// Une classe pour gérer la fin de la partie (victoire ou défaite).
/// </summary>
public class GestionFinPartie : MonoBehaviour
{
    public static GestionFinPartie Instance { get; private set; }

    [SerializeField] private FinDePartie finDePartie;  // référence au script FinDePartie

    private bool partieTerminee = false;
    public bool PartieTerminee => partieTerminee;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Une méthode pour déclarer la victoire.
    /// </summary>
    public void DeclarerVictoire()
    {
        if (partieTerminee) return;

        partieTerminee = true;
        if (finDePartie != null)
        {
            finDePartie.AfficherVictoire();
        }
    }

    /// <summary>
    /// Une methode pour déclarer la défaite.
    /// </summary>
    public void DeclarerDefaite()
    {
        if (partieTerminee) return;

        partieTerminee = true;
        if (finDePartie != null)
        {
            finDePartie.AfficherDefaite();
        }
    }
}
