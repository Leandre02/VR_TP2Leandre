using UnityEngine;

/// <summary>
/// Represente l'état de poursuite du drone
/// </summary>
public class DronePoursuiteState : IDroneState
{
    private Drone drone; // Le drone associé à cet état

    // Le constructeur de l'état de poursuite
    public DronePoursuiteState(Drone drone) 
    {
        this.drone = drone;
    }

    /// <summary>
    /// Entre dans l'état de poursuite et joue l'animation correspondante
    /// </summary>
    public void EntrerEtat()
    {
        drone.GetComponent<Animator>().Play("Patrouille");
    }

    public void Update()
    {
        if (drone.cible == null)
            return;

        drone.DeplacerVersCible(drone.cible.transform.position);
        drone.transform.LookAt(drone.cible.transform);

        float dist = Vector3.Distance(drone.transform.position, drone.cible.transform.position);
        if (dist > drone.distanceDetection)
        {
            drone.ChangerEtat(new DronePatrouilleState(drone)); // Passe à l'état de patrouille si la cible est hors de portée
        }
        else if (dist <= 3f)
        {
            drone.ChangerEtat(new DroneAttaqueState(drone)); // Passe à l'état d'attaque si la cible est à portée d'attaque
        }
    }
}
