using UnityEngine;
using System;

/// <summary>
/// Represente la gestion de la vie d'un personnage
/// </summary>
public class Vie : MonoBehaviour
{
    [SerializeField] private float pointsDeVieMax = 100f;
    public float PointsDeVieMax => pointsDeVieMax;

    public float PointsDeVie { get; private set; }

    private bool estMort = false;

    // Événement pour notifier les changements de vie (vie actuelle, vie max)
    public event Action<float, float> OnVieChange;

    void Awake()
    {
        PointsDeVie = pointsDeVieMax;
        OnVieChange?.Invoke(PointsDeVie, pointsDeVieMax);
    }

    public void PrendreDegats(float degats)
    {

        if (estMort) return;

        if (degats <= 0f) return;
        PointsDeVie = Mathf.Max(0f, PointsDeVie - degats);
        OnVieChange?.Invoke(PointsDeVie, pointsDeVieMax);
        if (PointsDeVie <= 0f) Mourir();
    }

    private void Mourir()
    {
        if (estMort) return;

        estMort = true;
        // Si c'est un drone qui meurt
        Drone drone = GetComponent<Drone>();
        if (drone != null)
        {
            drone.Exploser();
           
            drone.ChangerEtat(new DroneDestructionState(drone));
            return;

        }

        // Si c'est le joueur qui meurt

        if (CompareTag("Player"))
        {
            if (GestionFinPartie.Instance != null)
            {
                GestionFinPartie.Instance.DeclarerDefaite();
            }
            return;
        }

        Destroy(gameObject); // Detruit l'objet
    }
}
