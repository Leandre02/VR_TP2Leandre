using UnityEngine;
using System;

/// <summary>
/// Classe attachée à chaque cible pour gérer les points attribués lors de l'impact avec le marteau, ainsi que le son d'apparition
/// </summary>
public class Cible : MonoBehaviour
{
    [SerializeField] private int points = 10;
    [SerializeField] private AudioClip sonApparition;

    // Event C# — déclenché quand la cible est frappée par le marteau
    public static event Action<int> OnCibleTouchee;

    private AudioSource audioSource; // Variable pour jouer les sons

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // son spatial en VR

        // Joue le son d'apparition dès que la cible spawn
        if (sonApparition != null)
        {
            audioSource.PlayOneShot(sonApparition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifie si c'est le marteau qui touche
        if (other.CompareTag("Marteau"))
        {
            // Déclenche le feedback haptique et audio sur le marteau
            FeedbackMarteau feedback = other.GetComponent<FeedbackMarteau>();
            if (feedback != null)
            {
                feedback.DeclencherImpact();
            }

            // Annonce que la cible a été touchée avec ses points
            OnCibleTouchee?.Invoke(points);

            Debug.Log("Cible touchée ! +" + points + " points");
            Destroy(gameObject);
        }
    }
}