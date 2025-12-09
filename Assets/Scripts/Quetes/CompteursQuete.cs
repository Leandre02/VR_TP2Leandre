using UnityEngine;

/// <summary>
/// Gere les compteurs pour les objectifs des quetes
/// </summary>
public class CompteursQuete : MonoBehaviour
{
    public static CompteursQuete Instance { get; private set; }

    // Compteurs globaux
    public int dronesDetruits = 0;
    public bool valleTraversee = false;
    public bool meteoritesSurvecues = false;

    // Events pour notifier les quetes et autres systèmes
    public System.Action OnDroneDetruit;
    public System.Action OnValleTraversee;
    public System.Action OnMeteoriteSurvecue;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[CompteursQuete] Instance initialisée sur " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("[CompteursQuete] Instance déjà existante, destruction de " + gameObject.name);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Appelé quand un drone est détruit
    /// </summary>
    public void EnregistrerDroneDetruit()
    {
        dronesDetruits++;
        Debug.Log($"[CompteursQuete] Drone #{dronesDetruits}");

        // Notifie les quetes
        OnDroneDetruit?.Invoke();

        // Notifie le gestionnaire de phases
        if (GestionPhasesJeu.Instance != null)
        {
            GestionPhasesJeu.Instance.NotifierDroneDetruit();
        }
    }

    /// <summary>
    /// Appelé quand une météorite est détruite
    /// </summary>
    public void EnregistrerSurvieMeteorite()
    {
        if (!meteoritesSurvecues)
        {
            meteoritesSurvecues = true;
            Debug.Log("Meteorite survivee!");
            OnMeteoriteSurvecue?.Invoke();
        }

    }

    /// <summary>
    /// Appelé quand le joueur traverse la vallée
    /// </summary>
    public void EnregistrerValleTraversee()
    {
        if (!valleTraversee)
        {
            valleTraversee = true;
            Debug.Log("Vallee traversee!");
            OnValleTraversee?.Invoke();
        }
    }
}
