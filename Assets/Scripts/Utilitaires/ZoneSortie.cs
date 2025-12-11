using UnityEngine;

/// <summary>
/// Represente une zone de sortie que le joueur doit atteindre pour terminer le niveau.
/// </summary>
public class ZoneSortie : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GestionFinPartie.Instance != null)
        {
            if (!EstPhaseVictoire())
            {
                Debug.Log("[ZoneSortie] Le joueur atteint la zone, mais les objectifs ne sont pas remplis");
                return;
            }
            if (GestionFinPartie.Instance != null)
            {
                GestionFinPartie.Instance.DeclarerVictoire();
            }
        }
    }

    private bool EstPhaseVictoire()
    {
        return CompteursQuete.Instance != null
               && CompteursQuete.Instance.dronesDetruits >= 1;
    }

}
