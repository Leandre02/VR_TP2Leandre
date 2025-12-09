using UnityEngine;

/// <summary>
/// Represente l'état de destruction du drone
/// </summary>
public class DroneDestructionState : IDroneState
{
    private Drone drone; // Le drone associé à cet état

    // Le constructeur de l'état de destruction
    public DroneDestructionState(Drone drone)
    {
        this.drone = drone;
    }

    /// <summary>
    /// Permet d'entrer dans l'état de destruction
    /// </summary>
    public void EntrerEtat()
    {
        Debug.Log("[DroneDestructionState] EntrerEtat appelé pour " + drone.name);

        drone.ArreterDeplacement();
        drone.GetComponent<Animator>().SetTrigger("Detruit");

        // Désactive le collider pour éviter les interactions
        Collider col = drone.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Notifie le systeme de quetes
        if (CompteursQuete.Instance != null)
        {
            CompteursQuete.Instance.EnregistrerDroneDetruit();
            Debug.Log("[CompteursQuete] EnregistrerDroneDetruit appelé");

        }

        GameObject.Destroy(drone.gameObject, 4f);
    }

    public void Update()
    {
        Debug.Log("Drone a été détruit.");
    }
}
