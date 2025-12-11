using UnityEngine;

/// <summary>
/// Une classe pour gérer l'affichage de la fin de partie (victoire ou défaite).
/// </summary>
public class FinDePartie : MonoBehaviour
{
    [SerializeField] private GameObject panelVictoire;
    [SerializeField] private GameObject panelDefaite;
    [SerializeField] private GameObject canvasFin;

    /// <summary>
    /// Une méthode pour afficher l'écran de victoire.
    /// </summary>
    public void AfficherVictoire()
    {
        canvasFin.SetActive(true);
        panelVictoire.SetActive(true);
        panelDefaite.SetActive(false);
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Une méthode pour afficher l'écran de défaite.
    /// </summary>
    public void AfficherDefaite()
    {
        canvasFin.SetActive(true);
        panelVictoire.SetActive(false);
        panelDefaite.SetActive(true);
        Time.timeScale = 0f;
    }
}
