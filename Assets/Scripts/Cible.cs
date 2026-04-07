using UnityEngine;
using System;

public class Cible : MonoBehaviour
{
    public int points = 10;
    public AudioClip sonApparition;

    // Event C# — déclenché quand la cible est touchée
    public static event Action<int> OnCibleTouchee;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        if (sonApparition != null)
        {
            audioSource.PlayOneShot(sonApparition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Marteau"))
        {
            FeedbackMarteau feedback = other.GetComponent<FeedbackMarteau>();
            if (feedback != null)
            {
                feedback.DeclencherImpact(null);
            }

            // On déclenche l'event au lieu d'appeler directement
            OnCibleTouchee?.Invoke(points);

            Debug.Log("Cible touchée ! +" + points + " points");
            Destroy(gameObject);
        }
    }
}