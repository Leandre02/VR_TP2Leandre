using UnityEngine;
using System;

/// <summary>
/// Classe attachée à chaque cible pour gérer les points attribués lors de l'impact avec le marteau, ainsi que le son d'apparition
/// Inspiré des patterns d'événements C# et de communication GameManager vus dans les exercices d’UI VR / GameManager
/// et de feedback VR (haptiques et audio spatial), ainsi que des consignes du travail pratique Whack-a-Mole VR.
/// Références :
///  - Cégep de Victoriaville. Exercice 4.1 — Feedback VR : haptiques et audio spatial. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/exercice_feedback_vr/
///  - Cégep de Victoriaville. Exercice 4.2 — UI VR et GameManager. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/exercice_ui_jeu_tri/
///  - Cégep de Victoriaville. Travail pratique — Whack-a-Mole VR. Environnements Immersifs, 2026. https://envimmersif-cegepvicto.github.io/tp_sommatif1_vr/
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