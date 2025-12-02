using UnityEngine;

/// <summary>
/// Represente l'état d'attente du drone
/// </summary>
public class DroneIdleState : IDroneState
{
    private Drone drone; // Le drone associé à cet état

    // Le constructeur de l'état d'attente
    public DroneIdleState(Drone drone)
    {
        this.drone = drone;
    }

    /// <summary>
    /// Permet de d'entrer dans l'état d'attente et de jouer l'animation correspondante
    /// </summary>
    public void EntrerEtat()
    {
        var animator = drone.Animator;
        animator.ResetTrigger("Attaque");
        animator.ResetTrigger("Detruit");
        animator.SetBool("Patrouille", false);
    }


    public void Update()
    {
        if (drone.cible != null)
        {
            float dist = Vector3.Distance(drone.transform.position, drone.cible.transform.position);
            if (dist <= drone.distanceDetection)
            {
                drone.ChangerEtat(new DronePoursuiteState(drone)); // Passe à l'état de poursuite si la cible est détectée
            }
        }
    }
}
