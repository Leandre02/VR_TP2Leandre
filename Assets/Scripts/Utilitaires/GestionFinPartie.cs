using UnityEngine;

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

    public void DeclarerVictoire()
    {
        if (partieTerminee) return;

        partieTerminee = true;
        if (finDePartie != null)
        {
            finDePartie.AfficherVictoire();
        }
    }

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
