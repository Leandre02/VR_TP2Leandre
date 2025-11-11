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

    // Événement pour notifier les changements de vie (vie actuelle, vie max)
    public event Action<float, float> OnVieChange;

    void Awake()
    {
        PointsDeVie = pointsDeVieMax;
        OnVieChange?.Invoke(PointsDeVie, pointsDeVieMax);
    }

    public void PrendreDegats(float degats)
    {
        if (degats <= 0f) return;
        PointsDeVie = Mathf.Max(0f, PointsDeVie - degats);
        OnVieChange?.Invoke(PointsDeVie, pointsDeVieMax);
        if (PointsDeVie <= 0f) Mourir();
    }

    /// <summary>
    /// Une methode pour soigner le personnage
    /// </summary>
    /// <param name="valeur">La valeur portée par l'objet de guérison</param>
    public void Soigner(float valeur)
    {
        if (valeur <= 0f) return;
        PointsDeVie = Mathf.Min(pointsDeVieMax, PointsDeVie + valeur);
        OnVieChange?.Invoke(PointsDeVie, pointsDeVieMax);
    }

    private void Mourir()
    {
        
        Destroy(gameObject); // Detruit l'objet
    }
}
