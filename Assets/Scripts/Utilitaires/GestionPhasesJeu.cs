using UnityEngine;

public enum PhaseJeu
{
    Meteorites,
    Drones,
    Victoire
}

public class GestionPhasesJeu : MonoBehaviour
{
    public static GestionPhasesJeu Instance { get; private set; }

    [Header("Gestionnaires d'ennemis")]
    [SerializeField] private GameObject gestionnaireMeteorites; // GameObject avec SpawnerVagues
    [SerializeField] private GameObject gestionnaireDrones;     // GameObject avec SpawnerDrones


    [Header("Objectif quete drones")]
    [SerializeField] private int nbDronesPourVictoire = 1;

    private PhaseJeu phaseCourante = PhaseJeu.Meteorites;
    private int dronesDetruits = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        ActiverPhaseMeteorites();
    }

    private void ActiverPhaseMeteorites()
    {
        phaseCourante = PhaseJeu.Meteorites;

        if (gestionnaireMeteorites != null)
            gestionnaireMeteorites.SetActive(true);

        if (gestionnaireDrones != null)
            gestionnaireDrones.SetActive(false);

        dronesDetruits = 0;
    }

    public void PasserEnPhaseDrones()
    {
        if (phaseCourante != PhaseJeu.Meteorites)
            return;

        phaseCourante = PhaseJeu.Drones;

        if (gestionnaireMeteorites != null)
            gestionnaireMeteorites.SetActive(false); // stop la pluie

        if (gestionnaireDrones != null)
            gestionnaireDrones.SetActive(true); // active le spawner de drones
    }

    public void NotifierDroneDetruit()
    {
        if (phaseCourante != PhaseJeu.Drones)
            return;

        dronesDetruits++;

        if (dronesDetruits >= nbDronesPourVictoire)
        {
            ActiverVictoire();
        }
    }

    private void ActiverVictoire()
    {
        phaseCourante = PhaseJeu.Victoire;

        if (gestionnaireMeteorites != null)
            gestionnaireMeteorites.SetActive(false);

        if (gestionnaireDrones != null)
            gestionnaireDrones.SetActive(false);

    }
}
