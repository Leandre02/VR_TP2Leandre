using UnityEngine;

/// <summary>
/// Zone qui detecte quand le tank traverse la vallee
/// </summary>
public class ZoneVallee : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Verifie si c'est le tank
        if (other.CompareTag("Player"))
        {
            if (CompteursQuete.Instance != null)
            {
                CompteursQuete.Instance.EnregistrerValleTraversee();
                CompteursQuete.Instance.EnregistrerSurvieMeteorite();
            }


            if (GestionPhasesJeu.Instance != null)
            {
                GestionPhasesJeu.Instance.PasserEnPhaseDrones();
            }
        }
    }
}