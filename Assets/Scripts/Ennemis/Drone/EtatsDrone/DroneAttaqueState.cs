using UnityEngine;

/// <summary>
/// Represente l'état d'attaque du drone
/// </summary>
public class DroneAttaqueState : IDroneState
{
    private Drone drone; // Le drone associé à cet état
    private float dernierTempsAttaque; // Le temps de la dernière attaque
    private float delaiRecharge = 2f; // Le delai d'attente entre les attaques

    /// <summary>
    /// Le constructeur de l'état d'attaque
    /// </summary>
    /// <param name="drone"></param>
    public DroneAttaqueState(Drone drone)
    {
        this.drone = drone;
    }

    /// <summary>
    /// Une méthode appelée lors de l'entrée dans cet état
    /// Fait jouer l'animation d'attaque et initialise le temps de la dernière attaque
    /// </summary>
    public void EntrerEtat()
    {
        drone.GetComponent<Animator>().Play("Attaque");
        dernierTempsAttaque = -delaiRecharge;
    }

    /// <summary>
    /// Méthode appelée à chaque frame pour mettre à jour l'état
    /// Permet de gérer l'attaque de la cible et la transition vers d'autres états si nécessaire
    /// </summary>
    public void Update()
    {
        // Si la cible est perdue, revenir à l'état de patrouille
        if (drone.cible == null)
        {
            drone.ChangerEtat(new DronePatrouilleState(drone));
            return;
        }

        drone.transform.LookAt(drone.cible.transform); // Le drone regarde la cible

        if (Time.time >= dernierTempsAttaque + delaiRecharge)
        {
            Debug.Log("Drone attaque la cible !");
            dernierTempsAttaque = Time.time;
        }

        float distanceAttaque = Vector3.Distance(drone.transform.position, drone.cible.transform.position);
        if (distanceAttaque > 3f)
            drone.ChangerEtat(new DronePoursuiteState(drone)); // Si la distance d'attaque est dépassée, passe à l'état de poursuite
    }
}
