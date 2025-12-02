using UnityEngine;

public class FinDePartie : MonoBehaviour
{
    [SerializeField] private GameObject panelVictoire;
    [SerializeField] private GameObject panelDefaite;
    [SerializeField] private GameObject canvasFin;

    public void AfficherVictoire()
    {
        canvasFin.SetActive(true);
        panelVictoire.SetActive(true);
        panelDefaite.SetActive(false);
        Time.timeScale = 0f;
    }

    public void AfficherDefaite()
    {
        canvasFin.SetActive(true);
        panelVictoire.SetActive(false);
        panelDefaite.SetActive(true);
        Time.timeScale = 0f;
    }
}
