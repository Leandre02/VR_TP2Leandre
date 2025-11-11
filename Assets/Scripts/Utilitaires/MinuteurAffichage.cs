using TMPro;
using UnityEngine;

/// <summary>
/// Represente un minuteur affichant le temps restant au format MM:SS.
/// </summary>
public class MinuteurAffichage : MonoBehaviour
{
    public float temps = 120f; // Le temps initial en secondes
    public TextMeshProUGUI minuteurText; // Le composant TextMeshPro pour afficher le temps
    private bool actif = true;

    void Update()
    {
        if (!actif) return;

        temps -= Time.deltaTime;
        if (temps < 0)
        {
            temps = 0;
            actif = false;
        }

        int minutes = Mathf.FloorToInt(temps / 60);
        int secondes = Mathf.FloorToInt(temps % 60);

        minuteurText.text = string.Format("{0:00}:{1:00}", minutes, secondes);
    }
}
