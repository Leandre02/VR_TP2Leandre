using UnityEngine;

/// <summary>
/// Represente l'état de patrouille du drone
/// </summary>
public class DronePatrouilleState : IDroneState
{
    private Drone drone; // Le drone associé à cet état
    private Vector3 pointPatrouille; // Le point de patrouille actuel
    private bool pointDefini = false; // Indique si un point de patrouille est défini

    // Le constructeur de l'état de patrouille
    public DronePatrouilleState(Drone drone)
    {
        this.drone = drone;
    }

    /// <summary>
    /// Entre dans l'état de patrouille et joue l'animation correspondante
    /// </summary>
    public void EntrerEtat()
    {
        pointDefini = false;
        drone.GetComponent<Animator>().Play("Patrouille");
    }

    public void Update()
    {
        if (!pointDefini)
        {
            pointPatrouille = ObtenirPointPatrouille();
            pointDefini = true;
        }
        drone.DeplacerVersCible(pointPatrouille);

        if (Vector3.Distance(drone.transform.position, pointPatrouille) < 1f)
            pointDefini = false;

        if (drone.cible != null)
        {
            float dist = Vector3.Distance(drone.transform.position, drone.cible.transform.position);
            if (dist <= drone.distanceDetection)
            {
                drone.ChangerEtat(new DronePoursuiteState(drone));
            }
        }
    }

    /// <summary>
    /// Permet d'obtenir un point aléatoire de patrouille autour du drone
    /// </summary>
    /// <returns></returns>
    private Vector3 ObtenirPointPatrouille()
    {
        Vector3 posAleatoire = Random.insideUnitSphere * 10f + drone.transform.position;
        posAleatoire.y = drone.transform.position.y;
        return posAleatoire;
    }
}
